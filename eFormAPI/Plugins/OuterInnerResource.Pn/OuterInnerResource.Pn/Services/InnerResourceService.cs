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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eForm.Infrastructure.Constants;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Models.InnerResources;
using OuterInnerResource.Pn.Messages;
using Rebus.Bus;

namespace OuterInnerResource.Pn.Services
{
    using Microting.eFormApi.BasePn.Infrastructure.Helpers;

    public class InnerResourceService : IInnerResourceService
    {
        private readonly OuterInnerResourcePnDbContext _dbContext;
        private readonly IOuterInnerResourceLocalizationService _localizationService;
        private readonly ILogger<InnerResourceService> _logger;
        private readonly IEFormCoreService _coreService;
        private readonly IBus _bus;

        public InnerResourceService(OuterInnerResourcePnDbContext dbContext,
            IOuterInnerResourceLocalizationService localizationService,
            ILogger<InnerResourceService> logger,
            IEFormCoreService coreService,
            IRebusService rebusService)
        {
            _dbContext = dbContext;
            _localizationService = localizationService;
            _logger = logger;
            _coreService = coreService;
            _bus = rebusService.GetBus();
        }

        public async Task<OperationDataResult<InnerResourcesModel>> Index(InnerResourceRequestModel requestModel)
        {
            try
            {
                var innerResourcesModel = new InnerResourcesModel();

                var query = _dbContext.InnerResources
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                    .AsNoTracking()
                    .AsQueryable();

                query = QueryHelper.AddSortToQuery(query, requestModel.Sort, requestModel.IsSortDsc);

                innerResourcesModel.Total = await query.Select(x => x.Id).CountAsync();

                query = query
                    .Skip(requestModel.Offset)
                    .Take(requestModel.PageSize);

                innerResourcesModel.InnerResourceList = await query
                    .Select(x => new InnerResourceModel
                    {
                        Name = x.Name,
                        Id = x.Id,
                        ExternalId = x.ExternalId,
                        RelatedOuterResourcesIds = _dbContext.OuterInnerResources.Where(y =>
                            y.InnerResourceId == x.Id && y.WorkflowState != Constants.WorkflowStates.Removed).Select(z => z.OuterResourceId).ToList()
                    }).ToListAsync();

                innerResourcesModel.Name = _dbContext.PluginConfigurationValues
                    .FirstOrDefault(x => x.Name == "OuterInnerResourceSettings:InnerResourceName")
                    ?.Value;

                return new OperationDataResult<InnerResourcesModel>(true, innerResourcesModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<InnerResourcesModel>(false,
                    _localizationService.GetString("ErrorObtainInnerResources"));
            }
        }

        public async Task<OperationDataResult<InnerResourceModel>> Get(int innerResourceId)
        {
            try
            {
                var innerResource = await _dbContext.InnerResources.Select(x => new InnerResourceModel()
                {
                    Name = x.Name,
                    Id = x.Id,
                    ExternalId = x.ExternalId
                })
                    .FirstOrDefaultAsync(x => x.Id == innerResourceId);

                if (innerResource == null)
                {
                    return new OperationDataResult<InnerResourceModel>(false,
                        _localizationService.GetString("InnerResourceWithIdNotExist"));
                }

                var outerInnerResources = await _dbContext.OuterInnerResources
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed && x.InnerResourceId == innerResource.Id).AsNoTracking().ToListAsync();

                innerResource.RelatedOuterResourcesIds = new List<int>();
                foreach (var outerInnerResource in outerInnerResources)
                {
                    innerResource.RelatedOuterResourcesIds.Add(outerInnerResource.OuterResourceId);
                }

                return new OperationDataResult<InnerResourceModel>(true, innerResource);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<InnerResourceModel>(false,
                    _localizationService.GetString("ErrorObtainInnerResource"));
            }
        }

        public async Task<OperationResult> Create(InnerResourceModel model)
        {
            try
            {
                var innerResource = new InnerResource()
                {
                    Name = model.Name,
                    ExternalId = model.ExternalId,
                };

                await innerResource.Create(_dbContext);
                model.Id = innerResource.Id;

                if (model.RelatedOuterResourcesIds != null)
                {
                    foreach (var outerResourceId in model.RelatedOuterResourcesIds)
                    {
                        var macth = await _dbContext.OuterInnerResources.SingleOrDefaultAsync(x =>
                            x.InnerResourceId == model.Id
                            && x.OuterResourceId == outerResourceId);
                        if (macth == null)
                        {
                            var
                                outerInnerResource =
                                    new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.
                                        OuterInnerResource
                                    {
                                        OuterResourceId = outerResourceId,
                                        InnerResourceId = model.Id
                                    };
                            await outerInnerResource.Create(_dbContext);
                        }
                    }
                    await _bus.SendLocal(new OuterInnerResourceCreate(model, null));
                }

                return new OperationResult(true,
                    _localizationService.GetString("InnerResourceCreatedSuccessfully", model.Name));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString("ErrorCreatingInnerResource"));
            }
        }

        public async Task<OperationResult> Update(InnerResourceModel model)
        {
            try
            {
                var innerResource =
                    await _dbContext.InnerResources.SingleOrDefaultAsync(x => x.Id == model.Id);

                innerResource.ExternalId = model.ExternalId;
                innerResource.Name = model.Name;
                await innerResource.Update(_dbContext);

                var
                    outerInnerResources =
                        await _dbContext.OuterInnerResources.Where(x =>
                            x.InnerResourceId == innerResource.Id
                            && x.WorkflowState != Constants.WorkflowStates.Removed).ToListAsync();

                var requestedOuterResourceIds = model.RelatedOuterResourcesIds;
                var deployedOuterResourceIds = new List<int>();
                var toBeDeployed = new List<int>();

                foreach (var outerInnerResource in outerInnerResources)
                {
                    deployedOuterResourceIds.Add(outerInnerResource.OuterResourceId);

                    if (!model.RelatedOuterResourcesIds.Contains(outerInnerResource.OuterResourceId))
                    {
                        await outerInnerResource.Delete(_dbContext);
                        await _bus.SendLocal(new OuterInnerResourceUpdate(outerInnerResource.Id));
                    }
                }

                if (requestedOuterResourceIds.Count != 0)
                {
                    toBeDeployed.AddRange(requestedOuterResourceIds.Where(x =>
                        !deployedOuterResourceIds.Contains(x)));
                }

                foreach (var outerResourceId in toBeDeployed)
                {
                    var outerResource = _dbContext.OuterResources.SingleOrDefault(x =>
                        x.Id == outerResourceId);
                    if (outerResource != null)
                    {
                        var
                            outerInnerResource = await _dbContext.OuterInnerResources.SingleOrDefaultAsync(x =>
                            x.InnerResourceId == innerResource.Id
                            && x.OuterResourceId == outerResourceId);

                        if (outerInnerResource == null)
                        {
                            outerInnerResource =
                                new Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Entities.OuterInnerResource()
                                {
                                    OuterResourceId = outerResourceId,
                                    InnerResourceId = innerResource.Id
                                };
                            await outerInnerResource.Create(_dbContext);
                        }
                        else
                        {
                            outerInnerResource.WorkflowState = Constants.WorkflowStates.Created;
                            await outerInnerResource.Update(_dbContext);
                        }

                        await _bus.SendLocal(new OuterInnerResourceUpdate(outerInnerResource.Id));
                    }
                }
                return new OperationResult(true, _localizationService.GetString("InnerResourceUpdatedSuccessfully"));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString("ErrorUpdatingInnerResource"));
            }
        }

        public async Task<OperationResult> Delete(int innerResourceId)
        {
            try
            {
                var innerResource = new InnerResource()
                {
                    Id = innerResourceId
                };

                await innerResource.Delete(_dbContext);
                await _bus.SendLocal(new OuterInnerResourceDelete(new InnerResourceModel() { Id = innerResourceId }, null));
                return new OperationResult(true, _localizationService.GetString("InnerResourceDeletedSuccessfully"));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false,
                    _localizationService.GetString("ErrorWhileDeletingInnerResource"));
            }
        }
    }
}
