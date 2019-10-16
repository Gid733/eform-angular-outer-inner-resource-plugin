﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MachineArea.Pn.Infrastructure.Models;
using MachineArea.Pn.Infrastructure.Models.Settings;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Abstractions
{
    public interface IMachineAreaSettingsService
    {
        Task<OperationDataResult<MachineAreaBaseSettings>> GetSettings();
        Task<OperationResult> UpdateSettings(MachineAreaBaseSettings machineAreaSettingsModel);
        Task<OperationDataResult<List<int>>> GetSitesEnabled();
        Task<OperationResult> UpdateSitesEnabled(List<int> siteIds);
    }
}
