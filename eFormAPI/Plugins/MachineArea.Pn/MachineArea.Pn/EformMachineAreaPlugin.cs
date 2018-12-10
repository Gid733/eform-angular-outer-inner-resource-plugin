﻿using System.Collections.Generic;
using System.Reflection;
using MachineArea.Pn.Abstractions;
using MachineArea.Pn.Infrastructure.Data;
using MachineArea.Pn.Infrastructure.Data.Factories;
using MachineArea.Pn.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microting.eFormApi.BasePn;
using Microting.eFormApi.BasePn.Infrastructure.Models.Application;

namespace MachineArea.Pn
{
    public class EformMachineAreaPlugin : IEformPlugin
    {
        public string GetName() => "Microting MachineArea plugin";
        public string ConnectionStringName() => "EFormMachineAreaPnConnection";
        public string PluginPath() => PluginAssembly().Location;

        public Assembly PluginAssembly()
        {
            return typeof(EformMachineAreaPlugin).GetTypeInfo().Assembly;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMachineAreaLocalizationService, MachineAreaLocalizationService>();
            services.AddTransient<IAreaService, AreaService>();
            services.AddTransient<IMachineService, MachineService>();
        }

        public void ConfigureDbContext(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<MachineAreaPnDbContext>(o => o.UseSqlServer(connectionString,
                b => b.MigrationsAssembly(PluginAssembly().FullName)));

            MachineAreaPnContextFactory contextFactory = new MachineAreaPnContextFactory();
            using (MachineAreaPnDbContext context = contextFactory.CreateDbContext(new[] { connectionString }))
            {
                context.Database.Migrate();
            }

            // Seed database
            SeedDatabase(connectionString);
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
        }

        public MenuModel HeaderMenu()
        {
            var result = new MenuModel();
            result.LeftMenu.Add(new MenuItemModel()
            {
                Name = "Machine Area",
                E2EId = "",
                Link = "",
                MenuItems = new List<MenuItemModel>()
                {
                    new MenuItemModel()
                    {
                        Name = "Machines",
                        E2EId = "machine-area-pn-machines",
                        Link = "/plugins/machine-area-pn",
                        Position = 0,
                    },
                    new MenuItemModel()
                    {
                        Name = "Areas",
                        E2EId = "machine-area-pn-areas",
                        Link = "/plugins/machine-area-pn/areas",
                        Position = 1,
                    },
                    new MenuItemModel()
                    {
                        Name = "Settings",
                        E2EId = "case-management-pn-settings",
                        Link = "/plugins/case-management-pn/settings",
                        Position = 2,
                        Guards = new List<string>()
                        {
                            "admin"
                        }
                    },
                }
            });
            return result;
        }

        public void SeedDatabase(string connectionString)
        {
           
        }
    }
}