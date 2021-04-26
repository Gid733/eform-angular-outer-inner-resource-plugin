/*
The MIT License (MIT)

Copyright (c) 2007 - 2021 Microting A/S

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
    using eFormCore;
    using Infrastructure.Helpers;
    using Messages;
    using Microsoft.EntityFrameworkCore;
    using Microting.eForm.Infrastructure.Constants;
    using Microting.eForm.Infrastructure.Models;
    using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
    using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Constants;
    using Rebus.Handlers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class OuterInnerResourcePosteFormHandler : IHandleMessages<OuterInnerResourcePosteForm>
    {
        private readonly Core _core;
        private readonly OuterInnerResourcePnDbContext _dbContext;

        public OuterInnerResourcePosteFormHandler(Core core, DbContextHelper dbContextHelper)
        {
            _core = core;
            _dbContext = dbContextHelper.GetDbContext();
        }

        public async Task Handle(OuterInnerResourcePosteForm message)
        {
            var outerInnerResourceSite =
                await _dbContext.OuterInnerResourceSites.SingleOrDefaultAsync(x =>
                    x.Id == message.OuterInnerResourceSiteId);
            await using var sdkDbContext = _core.DbContextHelper.GetDbContext();
            var siteDto = await sdkDbContext.Sites.SingleAsync(x => x.Id == outerInnerResourceSite.MicrotingSdkSiteId);
            var language = await sdkDbContext.Languages.SingleAsync(x => x.Id == siteDto.LanguageId);
            var mainElement = await _core.ReadeForm(message.SdkeFormId, language);

            mainElement.Label = outerInnerResourceSite.OuterInnerResource.InnerResource.Name;
            mainElement.ElementList[0].Label = outerInnerResourceSite.OuterInnerResource.InnerResource.Name;
            mainElement.EndDate = DateTime.Now.AddYears(10).ToUniversalTime();
            mainElement.StartDate = DateTime.Now.ToUniversalTime();
            mainElement.Repeated = 0;

            var lookup = $"OuterInnerResourceSettings:{OuterInnerResourceSettingsEnum.QuickSyncEnabled}";

            var quickSyncEnabled = _dbContext.PluginConfigurationValues
                .AsNoTracking()
                .FirstOrDefault(x =>x.Name == lookup)?.Value == "true";

            if (quickSyncEnabled)
            {
                mainElement.EnableQuickSync = true;
            }
            
            var folderQuery = sdkDbContext.Folders
                .AsNoTracking()
                .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                .Where(x => x.FolderTranslations.Any(y =>
                    y.Name == outerInnerResourceSite.OuterInnerResource.OuterResource.Name))
                .Select(x => new {x.MicrotingUid, x.Id});
            var folder = await folderQuery
                .FirstOrDefaultAsync();
            
            if (folder == null)
            {
                var languages = await sdkDbContext.Languages
                    .AsNoTracking()
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                    .ToListAsync();
                var names = new List<KeyValuePair<string, string>>();
                var descriptions = new List<KeyValuePair<string, string>>();

                foreach (var languageLocal in languages)
                {
                    names.Add(new KeyValuePair<string, string>(languageLocal.LanguageCode, outerInnerResourceSite.OuterInnerResource.OuterResource.Name));
                    descriptions.Add(new KeyValuePair<string, string>(languageLocal.LanguageCode, ""));
                }

                await _core.FolderCreate(names, descriptions, null);
                
                folder = await folderQuery
                    .FirstOrDefaultAsync();
            }

            mainElement.CheckListFolderName = folder.MicrotingUid.ToString();

            var dataElement = (DataElement)mainElement.ElementList[0];

            dataElement.DataItemList.Add(new None(
                1,
                false,
                false,
                $"{outerInnerResourceSite.OuterInnerResource.OuterResource.Name} - {outerInnerResourceSite.OuterInnerResource.InnerResource.Name}",
                "",
                Constants.FieldColors.Default,
                -999,
                false));

            var sdkCaseId = await _core.CaseCreate(mainElement, "", (int)siteDto.MicrotingUid, folder.Id);

            outerInnerResourceSite.MicrotingSdkCaseId = sdkCaseId;
            await outerInnerResourceSite.Update(_dbContext).ConfigureAwait(false);
        }

    }
}