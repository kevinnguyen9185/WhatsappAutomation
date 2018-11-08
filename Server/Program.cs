using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore;
using Serilog;
using Serilog.Events;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args) 
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.RollingFile(
                Path.Combine(Directory.GetCurrentDirectory(), "Logs/Server-{Date}.log"),
                fileSizeLimitBytes: 1_000_000,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(2)
            )
            .CreateLogger();

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
	            .UseStartup<Startup>()
                .UseUrls("http://*:5552")
                .UseSerilog()
                .Build();
    }
}