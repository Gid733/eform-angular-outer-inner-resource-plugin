﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using eFormShared;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Data;
using MachineArea.Pn.Infrastructure.Data.Entities;
using MachineArea.Pn.Infrastructure.Models.Areas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Infrastructure.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace MachineArea.Pn.Services
{
    public class AreaService : IAreaService
    {
        private readonly MachineAreaPnDbContext _dbContext;
        private readonly IMachineAreaLocalizationService _localizationService;
        private readonly ILogger<AreaService> _logger;

        public AreaService(MachineAreaPnDbContext dbContext,
            IMachineAreaLocalizationService localizationService,
            ILogger<AreaService> logger)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
            _logger = logger;
        }

        public async Task<OperationDataResult<AreasModel>> GetAllAreas(AreaRequestModel requestModel)
        {
            try
            {
                var areasModel = new AreasModel();

                IQueryable<Area> areasQuery = _dbContext.Areas.AsQueryable();
                if (!string.IsNullOrEmpty(requestModel.Sort))
                {
                    if (requestModel.IsSortDsc)
                    {
                        areasQuery = areasQuery
                            .CustomOrderByDescending(requestModel.Sort);
                    }
                    else
                    {
                        areasQuery = areasQuery
                            .CustomOrderBy(requestModel.Sort);
                    }
                }
                else
                {
                    areasQuery = _dbContext.Areas
                        .OrderBy(x => x.Id);
                }

                if (requestModel.PageSize != null)
                {
                    areasQuery = areasQuery
                        .Skip(requestModel.Offset)
                        .Take((int)requestModel.PageSize);
                }

                


                var areas = await areasQuery.Select(x => new AreaModel()
                {
                    Name = x.Name,
                    Id = x.Id
                }).ToListAsync();

                areasModel.Total = await _dbContext.Areas.CountAsync();
                areasModel.AreaList = areas;

                return new OperationDataResult<AreasModel>(true, areasModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<AreasModel>(false,
                    _localizationService.GetString(""));
            }
        }

        public async Task<OperationDataResult<AreaModel>> GetSingleArea(int areaId)
        {
            try
            {
                AreaModel area = await _dbContext.Areas.Select(x => new AreaModel()
                    {
                        Name = x.Name,
                        Id = x.Id,
                        RelateMachinesIds = x.MachineAreas.Select(y => y.Machine.Id).ToList()
                    })
                    .FirstOrDefaultAsync(x => x.Id == areaId);

                if (area == null)
                {
                    return new OperationDataResult<AreaModel>(false,
                        _localizationService.GetString(""));
                }

                return new OperationDataResult<AreaModel>(true, area);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<AreaModel>(false,
                    _localizationService.GetString(""));
            }
        }

        public async Task<OperationResult> CreateArea(AreaCreateModel model)
        {
            try
            {
                var newArea = new Area()
                {
                    Name = model.Name,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = 1,
                    UpdatedByUserId = 2,
                    UpdatedAt = DateTime.UtcNow,
                    WorkflowState = Constants.WorkflowStates.Created,
                    MachineAreas = model.RelatedMachinesIds
                        .Select(x => new Infrastructure.Data.Entities.MachineArea()
                        {
                            MachineId = x
                        }).ToList()
                };

                await _dbContext.Areas.AddAsync(newArea);
                await _dbContext.SaveChangesAsync();
                return new OperationResult(true, _localizationService.GetString(""));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString(""));
            }
        }

        public async Task<OperationResult> UpdateArea(AreaUpdateModel model)
        {
            try
            {
                var areaForUpdate = await _dbContext.Areas.FirstOrDefaultAsync(x => x.Id == model.Id);

                if (areaForUpdate == null)
                {
                    return new OperationResult(false,
                        _localizationService.GetString(""));
                }

                areaForUpdate.Name = model.Name;
                areaForUpdate.WorkflowState = Constants.WorkflowStates.Processed;
                areaForUpdate.UpdatedByUserId = 2;
                areaForUpdate.UpdatedAt = DateTime.UtcNow;

                var areasForDelete = await _dbContext.MachineAreas
                    .Where(x => !model.RelateMachinesIds.Contains(x.MachineId) && x.AreaId == model.Id)
                    .ToListAsync();

                var machineIds = await _dbContext.MachineAreas
                    .Where(x => model.RelateMachinesIds.Contains(x.MachineId) && x.AreaId == model.Id)
                    .Select(x => x.MachineId)
                    .ToListAsync();

                _dbContext.RemoveRange(areasForDelete);

                foreach (var machineId in model.RelateMachinesIds)
                {
                    if (!machineIds.Contains(machineId))
                    {
                        areaForUpdate.MachineAreas.Add(new Infrastructure.Data.Entities.MachineArea()
                        {
                            AreaId = model.Id,
                            MachineId = machineId
                        });
                    }
                }

                await _dbContext.SaveChangesAsync();
                return new OperationResult(true, _localizationService.GetString(""));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString(""));
            }
        }

        public async Task<OperationResult> DeleteArea(int areaId)
        {
            try
            {
                var areaForDelete = await _dbContext.Areas.FirstOrDefaultAsync(x => x.Id == areaId);

                if (areaForDelete == null)
                {
                    return new OperationResult(false,
                        _localizationService.GetString(""));
                }

                _dbContext.Areas.Remove(areaForDelete);
                await _dbContext.SaveChangesAsync();
                return new OperationResult(true, _localizationService.GetString(""));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString(""));
            }
        }
    }
}
