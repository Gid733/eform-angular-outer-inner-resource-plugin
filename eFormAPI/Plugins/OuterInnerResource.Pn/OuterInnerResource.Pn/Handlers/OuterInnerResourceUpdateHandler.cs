/*
The MIT License (MIT)

Copyright (c) 2007 - 2019 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/


namespace OuterInnerResource.Pn.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using eFormCore;
    using Infrastructure.Helpers;
    using Messages;
    using Microsoft.EntityFrameworkCore;
    using Microting.eForm.Dto;
    using Microting.eForm.Infrastructure.Constants;
    using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
    using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Constants;
    using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
    using Rebus.Bus;
    using Rebus.Handlers;

    public class OuterInnerResourceUpdateHandler : IHandleMessages<OuterInnerResourceUpdate>
    {  
        private readonly Core _core;
        private readonly OuterInnerResourcePnDbContext _dbContext;  
        private readonly IBus _bus;      
        
        public OuterInnerResourceUpdateHandler(Core core, DbContextHelper dbContextHelper, IBus bus)
        {
            _core = core;
            _dbContext = dbContextHelper.GetDbContext();
            _bus = bus;
        }
        
        #pragma warning disable 1998
        public async Task Handle(OuterInnerResourceUpdate message)
        {            
            var lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.SdkeFormId.ToString()}";

            var result = _dbContext.PluginConfigurationValues.AsNoTracking()
                .FirstOrDefault(x => 
                    x.Name == lookup)?.Value;
            if (int.TryParse(result, out var eFormId))
            {
                
                var sites = new List<SiteDto>();
            
                lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.EnabledSiteIds.ToString()}"; 
                result = _dbContext.PluginConfigurationValues.AsNoTracking()
                    .FirstOrDefault(x => 
                        x.Name == lookup)?.Value;
                if (result != null)
                {
                    var sdkSiteIds = result;
                    foreach (var siteId in sdkSiteIds.Split(","))
                    {
                        if (int.TryParse(siteId, out var siteIdResultParse))
                        {
                            sites.Add(await _core.SiteRead(siteIdResultParse));
                        }
                    }

                    var outerInnerResource =
                        await _dbContext.OuterInnerResources.SingleOrDefaultAsync(x => 
                            x.Id == message.OuterInnerResourceId);

                    await UpdateSitesDeployed(outerInnerResource, sites, eFormId);
                }
            }
        }

        private async Task UpdateSitesDeployed(
            OuterInnerResource outerInnerResource, List<SiteDto> sites, int eFormId)
        {

            WriteLogEntry("OuterInnerResourceUpdateHandler: UpdateSitesDeployed called");
            var siteIds = new List<int>();
            
            if (outerInnerResource.WorkflowState == Constants.WorkflowStates.Created)
            {
                if (sites.Any())
                {
                    foreach (var siteDto in sites)
                    {
                        siteIds.Add(siteDto.SiteId);
                        var outerInnerResourceSites = await _dbContext.OuterInnerResourceSites.Where(
                            x =>
                                x.MicrotingSdkSiteId == siteDto.SiteId
                                && x.OuterInnerResourceId == outerInnerResource.Id
                                && x.WorkflowState == Constants.WorkflowStates.Created).ToListAsync();
                        if (!outerInnerResourceSites.Any())
                        {
                            var outerInnerResourceSite = new OuterInnerResourceSite
                            {
                                OuterInnerResourceId = outerInnerResource.Id,
                                MicrotingSdkSiteId = siteDto.SiteId,
                                MicrotingSdkeFormId = eFormId
                            };
                            await outerInnerResourceSite.Create(_dbContext);
                            await _bus.SendLocal(new OuterInnerResourcePosteForm(outerInnerResourceSite.Id,
                                eFormId));
                        }
                        else
                        {
                            if (outerInnerResourceSites.First().MicrotingSdkCaseId == null)
                            {
                                await _bus.SendLocal(new OuterInnerResourcePosteForm(
                                    outerInnerResourceSites.First().Id,
                                    eFormId));
                            }
                        }
                    }
                } 
            }
            var sitesConfigured = _dbContext.OuterInnerResourceSites.Where(x => 
                x.OuterInnerResourceId == outerInnerResource.Id 
                && x.WorkflowState != Constants.WorkflowStates.Removed).ToList();
            WriteLogEntry("OuterInnerResourceUpdateHandler: sitesConfigured looked up");

            if (sitesConfigured.Any())
            {
                foreach (var outerInnerResourceSite in sitesConfigured)
                {
                    if (!siteIds.Contains(outerInnerResourceSite.MicrotingSdkSiteId) 
                        || outerInnerResource.WorkflowState == Constants.WorkflowStates.Removed)
                    {
                        if (outerInnerResourceSite.MicrotingSdkCaseId != null)
                        {
                            await outerInnerResourceSite.Delete(_dbContext);
                            await _bus.SendLocal(new OuterInnerResourceDeleteFromServer(outerInnerResourceSite.Id));
                        }
                    }
                }    
            }
        }

        private void WriteLogEntry(string message)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[DBG] " + message);
            Console.ForegroundColor = oldColor;
        }
    }
}