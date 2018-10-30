using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApi.Business;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            var selFolderOtions = app.Option("--selfolder <selfolder>", "add folder", CommandOptionType.SingleValue);
            app.OnExecute(() => {
                if(selFolderOtions.HasValue()){
                    GlobalVariable.SharedSelFolder = selFolderOtions.Value();
                    Console.WriteLine($"Set selfolder to {GlobalVariable.SharedSelFolder}");
                }
                return 0;
            });
            app.Execute(args);
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:5551")
                .UseKestrel(options =>
                {
                    // Set properties and call methods on options
                });
    }
}
