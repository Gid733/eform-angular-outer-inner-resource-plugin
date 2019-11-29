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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eFormCore;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Dto;
using Microting.eForm.Infrastructure.Models;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Constants;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using OuterInnerResource.Pn.Infrastructure.Helpers;
using OuterInnerResource.Pn.Infrastructure.Models.InnerResources;
using OuterInnerResource.Pn.Infrastructure.Models.OuterResources;
using OuterInnerResource.Pn.Messages;
using Rebus.Handlers;

namespace OuterInnerResource.Pn.Handlers
{
    public class OuterInnerResourceCreateHandler : IHandleMessages<OuterInnerResourceCreate>
    {        
        private readonly Core _core;
        private readonly OuterInnerResourcePnDbContext _dbContext;        
        
        public OuterInnerResourceCreateHandler(Core core, DbContextHelper dbContextHelper)
        {
            _core = core;
            _dbContext = dbContextHelper.GetDbContext();
        }
        
        #pragma warning disable 1998
        public async Task Handle(OuterInnerResourceCreate message)
        {            
            string lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.SdkeFormId.ToString()}"; 
            
            LogEvent($"lookup is {lookup}");

            string result = _dbContext.PluginConfigurationValues.AsNoTracking()
                .FirstOrDefault(x =>
                    x.Name == lookup)
                ?.Value;
            
            LogEvent($"result is {result}");
            
            int eFormId = int.Parse(result);

            MainElement mainElement = await _core.TemplateRead(eFormId);
            List<Site_Dto> sites = new List<Site_Dto>();
            
            lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.EnabledSiteIds.ToString()}"; 
            LogEvent($"lookup is {lookup}");

            string sdkSiteIds = _dbContext.PluginConfigurationValues.AsNoTracking()
                .FirstOrDefault(x => 
                    x.Name == lookup)?.Value;


            if (sdkSiteIds != null)
            {
                LogEvent($"sdkSiteIds is {sdkSiteIds}");
                foreach (string siteId in sdkSiteIds.Split(","))
                {
                    LogEvent($"found siteId {siteId}");
                    sites.Add(await _core.SiteRead(int.Parse(siteId)));
                }
            }

            if (message.InnerResourceModel != null)
            {
                await CreateFromMachine(message.InnerResourceModel, mainElement, sites, eFormId);
            }
            else
            {
                await CreateFromArea(message.OuterResourceModel, mainElement, sites, eFormId);
            }            
        }

        private async Task CreateFromMachine(InnerResourceModel model, MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {
            if (model.RelatedOuterResourcesIds != null)
            {
                foreach (int areaId in model.RelatedOuterResourcesIds)
                {                
                    OuterResource area = _dbContext.OuterResources.SingleOrDefault(x => x.Id == areaId);
                    await CreateRelationships(model.Id, areaId, model.Name, area.Name, mainElement, sites, eFormId);              
                }    
            }
        }

        private async Task CreateFromArea(OuterResourceModel model, MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {
            if (model.RelatedInnerResourcesIds != null)
            {
                foreach (int machineId in model.RelatedInnerResourcesIds)
                {
                    InnerResource machine = _dbContext.InnerResources.SingleOrDefault(x => x.Id == machineId);
                    await CreateRelationships(machineId, model.Id, machine.Name, model.Name, mainElement, sites, eFormId);
                }    
            }
        }

        private async Task CreateRelationships(int machineId, int areaId, string machineName, string areaName, MainElement mainElement, List<Site_Dto> sites, int eFormId)
        {
            Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource match = _dbContext.OuterInnerResources.SingleOrDefault(x =>
                    x.InnerResourceId == machineId && x.OuterResourceId == areaId);
            if (match == null)
            {
                Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource machineArea =
                    new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource();
                machineArea.OuterResourceId = areaId;
                machineArea.InnerResourceId = machineId;
                await machineArea.Create(_dbContext);
                mainElement.Label = machineName;
                mainElement.ElementList[0].Label = machineName;
                mainElement.EndDate = DateTime.Now.AddYears(10).ToUniversalTime();
                mainElement.StartDate = DateTime.Now.ToUniversalTime();
                mainElement.Repeated = 0;

                string lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.QuickSyncEnabled.ToString()}"; 
                LogEvent($"lookup is {lookup}");

                bool quickSyncEnabled = _dbContext.PluginConfigurationValues.AsNoTracking()
                    .FirstOrDefault(x => 
                        x.Name == lookup)?.Value == "true";

                if (quickSyncEnabled)
                {
                    mainElement.EnableQuickSync = true;    
                }

                List<Folder_Dto> folderDtos = await _core.FolderGetAll(true);

                bool folderAlreadyExist = false;
                int microtingUId = 0;
                foreach (Folder_Dto folderDto in folderDtos)
                {
                    if (folderDto.Name == areaName)
                    {
                        folderAlreadyExist = true;
                        microtingUId = (int)folderDto.MicrotingUId;
                    }
                }

                if (!folderAlreadyExist)
                {
                    await _core.FolderCreate(areaName, "", null);
                    folderDtos = await _core.FolderGetAll(true);
                
                    foreach (Folder_Dto folderDto in folderDtos)
                    {
                        if (folderDto.Name == areaName)
                        {
                            microtingUId = (int)folderDto.MicrotingUId;
                        }
                    }
                }
                
                mainElement.CheckListFolderName = microtingUId.ToString();
                
                foreach (Site_Dto siteDto in sites)
                {
                    OuterInnerResourceSite siteMatch = _dbContext.OuterInnerResourceSites.SingleOrDefault(x =>
                        x.MicrotingSdkSiteId == siteDto.SiteId && x.OuterInnerResourceId == machineArea.Id);
                    if (siteMatch == null)
                    {
                        int? sdkCaseId = await _core.CaseCreate(mainElement, "", siteDto.SiteId);

                        if (sdkCaseId != null)
                        {
                            OuterInnerResourceSite machineAreaSite = new OuterInnerResourceSite();
                            machineAreaSite.OuterInnerResourceId = machineArea.Id;
                            machineAreaSite.MicrotingSdkSiteId = siteDto.SiteId;
                            machineAreaSite.MicrotingSdkCaseId = (int)sdkCaseId;
                            machineAreaSite.MicrotingSdkeFormId = eFormId;
                            await machineAreaSite.Create(_dbContext);
                        }    
                    }
                }    
            }     
        }
        
        private void LogEvent(string appendText)
        {
            try
            {                
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("[DBG] " + appendText);
                Console.ForegroundColor = oldColor;
            }
            catch
            {
            }
        }

        private void LogException(string appendText)
        {
            try
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERR] " + appendText);
                Console.ForegroundColor = oldColor;
            }
            catch
            {

            }
        }
    }
}