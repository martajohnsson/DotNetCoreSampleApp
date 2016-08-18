﻿using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        #region Native IoC Container

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAttendeeService, AttendeeService>();
            //services.AddTransient<ISessionService, SessionService>();

            // Add framework services.
            services.AddMvc();

            // Add functionality to inject IOptions<T>.
            services.AddOptions();

            // Add our config object, so that it can be injected.
            services.Configure<Attendee>(Configuration.GetSection("AttendeeInfo"));
        }

        #endregion

        #region 3rd Party IoC Container

        //public IServiceProvider ConfigureServices(IServiceCollection services)
        //{
        //    // Add framework services.
        //    services.AddMvc();

        //    // Add Autofac
        //    var containerBuilder = new ContainerBuilder();
        //    containerBuilder.RegisterModule<DefaultModule>();
        //    containerBuilder.Populate(services);
        //    var container = containerBuilder.Build();
        //    return container.Resolve<IServiceProvider>();
        //}

        #endregion

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}