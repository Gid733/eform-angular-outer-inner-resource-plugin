using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microting.eFormApi.BasePn;
using Microting.eFormApi.BasePn.Infrastructure.Database.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Models.Application;
using Microting.eFormApi.BasePn.Infrastructure.Settings;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data;
using Microting.eFormOuterInnerResourceBase.Infrastructure.Data.Factories;
using OuterInnerResource.Pn.Abstractions;
using OuterInnerResource.Pn.Infrastructure.Data.Seed;
using OuterInnerResource.Pn.Infrastructure.Data.Seed.Data;
using OuterInnerResource.Pn.Infrastructure.Models.Settings;
using OuterInnerResource.Pn.Services;

namespace OuterInnerResource.Pn
{
    public class EformOuterInnerResourcePlugin : IEformPlugin
    {
        public string Name => "Microting Outer Inner Resource plugin";
        public string PluginId => "eform-angular-outer-inner-resource-plugin";
        public string PluginPath => PluginAssembly().Location;
        private string _connectionString;
        private string _outerResourceName = "OuterResources";
        private string _innerResourceName = "InnerResources";

        public Assembly PluginAssembly()
        {
            return typeof(EformOuterInnerResourcePlugin).GetTypeInfo().Assembly;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IOuterInnerResourceLocalizationService, OuterInnerResourceLocalizationService>();
            services.AddTransient<IOuterResourceService, OuterResourceService>();
            services.AddTransient<IInnerResourceService, InnerResourceService>();
            services.AddTransient<IOuterInnerResourceSettingsService, OuterInnerResourceSettingsService>();
            services.AddTransient<IOuterInnerResourceReportService, OuterInnerResourceReportService>();
            services.AddTransient<IResourceTimeRegistrationService, ResourceTimeRegistrationService>();
            services.AddTransient<IExcelService, ExcelService>();
            services.AddSingleton<IRebusService, RebusService>();
        }

        public void AddPluginConfig(IConfigurationBuilder builder, string connectionString)
        {
            OuterInnerResourceConfigurationSeedData seedData = new OuterInnerResourceConfigurationSeedData();
            OuterInnerResourcePnContextFactory contextFactory = new OuterInnerResourcePnContextFactory();
            builder.AddPluginConfiguration(
                connectionString,
                seedData,
                contextFactory);
        }

        public void ConfigureOptionsServices(
            IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigurePluginDbOptions<MachineAreaBaseSettings>(
                configuration.GetSection("MachineAreaBaseSettings"));
        }

        public void ConfigureDbContext(IServiceCollection services, string connectionString)
        {
            _connectionString = connectionString;
            if (connectionString.ToLower().Contains("convert zero datetime"))
            {
                services.AddDbContext<OuterInnerResourcePnDbContext>(o => o.UseMySql(connectionString,
                    b => b.MigrationsAssembly(PluginAssembly().FullName)));
            }
            else
            {
                services.AddDbContext<OuterInnerResourcePnDbContext>(o => o.UseSqlServer(connectionString,
                    b => b.MigrationsAssembly(PluginAssembly().FullName)));
            }

            OuterInnerResourcePnContextFactory contextFactory = new OuterInnerResourcePnContextFactory();

            using (OuterInnerResourcePnDbContext context = contextFactory.CreateDbContext(new[] {connectionString}))
            {  
                context.Database.Migrate();
                try
                {
                    _outerResourceName = context.PluginConfigurationValues.SingleOrDefault(x => x.Name == "MachineAreaBaseSettings:OuterResourceName")?.Value;
                    _innerResourceName = context.PluginConfigurationValues.SingleOrDefault(x => x.Name == "MachineAreaBaseSettings:InnerResourceName")?.Value;    
                } catch {}
                
            }

            // Seed database
            SeedDatabase(connectionString);
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            IServiceProvider serviceProvider = appBuilder.ApplicationServices;
            IRebusService rebusService = serviceProvider.GetService<IRebusService>();
            rebusService.Start(_connectionString);
        }

        public MenuModel HeaderMenu(IServiceProvider serviceProvider)
        {
            IOuterInnerResourceLocalizationService localizationService = serviceProvider
                .GetService<IOuterInnerResourceLocalizationService>();

            MenuModel result = new MenuModel();
            result.LeftMenu.Add(new MenuItemModel()
            {
                Name = localizationService.GetString("OuterInnerResource"),
                E2EId = "outer-inner-resource-pn",
                Link = "",
                MenuItems = new List<MenuItemModel>()
                {
                    new MenuItemModel()
                    {
//                        Name = localizationService.GetString("Machines"),
                        Name = _innerResourceName,
                        E2EId = $"outer-inner-resource-pn-inner-resources",
                        Link = $"/plugins/outer-inner-resource-pn/inner-resources",
                        Position = 0,
                    },
                    new MenuItemModel()
                    {
//                        Name = localizationService.GetString("Areas"),
                        Name = _outerResourceName,
                        E2EId = $"outer-inner-resource-pn-outer-resources",
                        Link = $"/plugins/outer-inner-resource-pn/outer-resources",
                        Position = 1,
                    },
                    new MenuItemModel()
                    {
                        Name = localizationService.GetString("Reports"),
                        E2EId = "outer-inner-resource-pn-reports",
                        Link = "/plugins/outer-inner-resource-pn/reports",
                        Position = 2,
                    },
                    new MenuItemModel()
                    {
                        Name = localizationService.GetString("Settings"),
                        E2EId = "outer-inner-resource-pn-settings",
                        Link = "/plugins/outer-inner-resource-pn/settings",
                        Position = 3,
                    }
                }
            });
            return result;
        }

        public void SeedDatabase(string connectionString)
        {
            // Get DbContext
            OuterInnerResourcePnContextFactory contextFactory = new OuterInnerResourcePnContextFactory();
            using (OuterInnerResourcePnDbContext context = contextFactory.CreateDbContext(new[] { connectionString }))
            {
                // Seed configuration
                OuterInnerResourcePluginSeed.SeedData(context);
            }
        }
    }
}