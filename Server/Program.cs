using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args) 
        { 
            var host = new WebHostBuilder() 
                .UseKestrel() 
                .UseContentRoot(Directory.GetCurrentDirectory()) 
                .UseIISIntegration() 
                .UseStartup<Startup>()
                .Build(); 
 
            host.Run(); 
        }
    }
}