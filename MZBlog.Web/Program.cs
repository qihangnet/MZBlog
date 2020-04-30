using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace MZBlog.Web
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var configuration = InitConfigration();

            Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .CreateLogger();

            try
            {
                var dbUpSuccess = Data.DbSetup.UseConfiguration(configuration).Upgrade("mzblog.db");

                if (dbUpSuccess)
                {
                    Log.Information("Starting web host");
                    CreateHostBuilder(configuration, args).Build().Run();
                }
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseConfiguration(configuration);
                webBuilder.UseStartup<Startup>();
                webBuilder.UseSerilog();
            });

        private static IConfigurationRoot InitConfigration()
        {
            return new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                                .AddEnvironmentVariables()
                                .Build();
        }
    }
}