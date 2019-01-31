﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Models.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormMachineAreaBase.Infrastructure.Data;

namespace MachineArea.Pn.Services
{
    public class MachineAreaReportService : IMachineAreaReportService
    {
        private readonly ILogger<MachineAreaReportService> _logger;
        private readonly IMachineAreaLocalizationService _machineAreaLocalizationService;
        private readonly MachineAreaPnDbContext _dbContext;
        private readonly IEFormCoreService _coreHelper;

        public MachineAreaReportService(ILogger<MachineAreaReportService> logger,
            MachineAreaPnDbContext dbContext,
            IEFormCoreService coreHelper,
            IMachineAreaLocalizationService machineAreaLocalizationService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _coreHelper = coreHelper;
            _machineAreaLocalizationService = machineAreaLocalizationService;
        }

        public async Task<OperationDataResult<ReportModel>> GenerateReport(GenerateReportModel model)
        {
            try
            {
                 var core = _coreHelper.GetCore();
                 var sitesList = core.SiteReadAll(false);

                 var jobsList = await _dbContext.MachineAreaTimeRegistrations.ToListAsync();
                 var reportEntitiesList = new List<ReportEntityModel>();
                 
                 switch (model.Type)
                 {
                     case 1:
                         reportEntitiesList = jobsList.GroupBy(x => x.SDKSiteId)
                             .Select(x => new ReportEntityModel()
                             {
                                 EntityName = sitesList.FirstOrDefault(y => y.SiteId == x.Key)?.SiteName,
                                 EntityId = x.Key,
                                 TimePerTimeUnit = x.GroupBy(d => d.DoneAt.Month).Select(g => g.Sum(s => s.TimeInMinutes)).ToList(),
                                 TotalTime = x.Sum(z => z.TimeInMinutes)
                             })
                             .ToList();
                         break;
                     case 2:
                         reportEntitiesList = jobsList.GroupBy(x => x.SDKSiteId)
                             .Select(x => new ReportEntityModel()
                             {
                                 EntityName = sitesList.FirstOrDefault(y => y.SiteId == x.Key)?.SiteName,
                                 EntityId = x.Key,
                                 TimePerTimeUnit = x.GroupBy(d => DateTimeFormatInfo.CurrentInfo.Calendar
                                     .GetWeekOfYear(d.DoneAt, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                                     .Select(g => g.Sum(s => s.TimeInMinutes)).ToList(),
                                 TotalTime = x.Sum(z => z.TimeInMinutes)
                             })
                             .ToList();
                         break;
                     case 3:
                         reportEntitiesList = jobsList.GroupBy(x => x.SDKSiteId)
                             .Select(x => new ReportEntityModel()
                             {
                                 EntityName = sitesList.FirstOrDefault(y => y.SiteId == x.Key)?.SiteName,
                                 EntityId = x.Key,
                                 TimePerTimeUnit = x.GroupBy(d => d.DoneAt.Day).Select(g => g.Sum(s => s.TimeInMinutes)).ToList(),
                                 TotalTime = x.Sum(z => z.TimeInMinutes)
                             })
                             .ToList();
                         break;
                 }
                 
                 var sumByTimeUnit = new List<int>();

                 foreach (var reportEntity in reportEntitiesList)
                 {
                     var i = 0;
                     foreach (var timePerTimeUnit in reportEntity.TimePerTimeUnit)
                     {
                         if (sumByTimeUnit.Count <= i)
                         {
                             sumByTimeUnit.Add(timePerTimeUnit);
                         }
                         else
                         {
                             sumByTimeUnit[i] += timePerTimeUnit;
                         }
                         i++;
                     }
                 }
                 
                 var finalModel = new ReportModel()
                 {
                     Entities = reportEntitiesList,
                     ReportHeaders = new List<ReportEntityHeaderModel>(),
                     TotalTime = reportEntitiesList.Sum(x => x.TotalTime),
                     TotalTimePerTimeUnit = sumByTimeUnit
                 };

                return new OperationDataResult<ReportModel>(true, finalModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<ReportModel>(false,
                    _machineAreaLocalizationService.GetString(""));
            }
        }

        public async Task<OperationResult> GenerateReportFile(GenerateReportModel model)
        {
            try
            {
                
                return new OperationResult(true,
                    _machineAreaLocalizationService.GetString(""));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _machineAreaLocalizationService.GetString(""));
            }
        }

    }
}
