using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Server.WebSocketManager;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Server.ServerSocket;

namespace Server 
{ 
    public class Startup 
    { 
        // This method gets called by the runtime. Use this method to add services to the container. 
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940 
        public void ConfigureServices(IServiceCollection services) 
        { 
            // services.AddSingleton(new LoggerFactory()
            //     .AddConsole());
            // services.AddLogging();
            services.AddCors(options=>{
                options.AddPolicy("localhost",
                    builder=> builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddMvc(); 
            services.AddWebSocketManager(); 
        } 
 
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. 
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider) 
        { 
            if (env.IsDevelopment()) 
            { 
                app.UseDeveloperExceptionPage(); 
            } 
 
            app.UseWebSockets();

            app.MapWebSocketManager("/ServerSocket", serviceProvider.GetService<ServerSocketHandler>());
 
            app.UseMvcWithDefaultRoute();
        }
    } 
} 