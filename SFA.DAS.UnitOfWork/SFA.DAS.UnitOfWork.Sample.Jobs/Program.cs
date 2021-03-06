﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.UnitOfWork.EntityFrameworkCore;
using SFA.DAS.UnitOfWork.EntityFrameworkCore.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.NServiceBus;
using SFA.DAS.UnitOfWork.NServiceBus.DependencyResolution;
using SFA.DAS.UnitOfWork.NServiceBus.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.Sample.Data;
using SFA.DAS.UnitOfWork.Sample.Jobs.Startup;
using SFA.DAS.UnitOfWork.Sample.Startup;

namespace SFA.DAS.UnitOfWork.Sample.Jobs
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .UseEnvironment(Environment.GetEnvironmentVariable(HostDefaults.EnvironmentKey))
                .ConfigureAppConfiguration((c, b) => b
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{c.HostingEnvironment.EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args))
                .ConfigureLogging(b => b.AddConsole())
                .UseNServiceBusContainer()
                .ConfigureServices((c, s) => s
                    .AddSqlServer(c.Configuration)
                    .AddEntityFramework()
                    .AddEntityFrameworkUnitOfWork<SampleDbContext>()
                    .AddNServiceBusUnitOfWork())
                .ConfigureContainer<UpdateableServiceProvider>((c, s) => s.StartNServiceBus(c.Configuration));
    }
}