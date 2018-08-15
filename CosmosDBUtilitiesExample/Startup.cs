﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CosmosDBUtilitiesExample.Data;
using CosmosDBUtilitiesExample.Models;
using CosmosDBUtilitiesExample.Services;
using MvcControlsToolkit.Business.DocumentDB;
using Microsoft.AspNetCore.Http;
using Project.Data.Data;
using Project.Data.Repository;

namespace CosmosDBUtilitiesExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddSingleton<IDocumentDBConnection>(x =>
                CosmosDBDefinitions.GetConnection()
                );
            services.AddTransient<ToDoItemsRepository>(x =>
                new ToDoItemsRepository(
                    x.GetService<IDocumentDBConnection>(),
                    x.GetService<IHttpContextAccessor>()
                        .HttpContext?.User?.Identity?.Name
                    ));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //var res=CosmosDBDefinitions.Initialize();
            //res.Wait();
        }
    }
}
