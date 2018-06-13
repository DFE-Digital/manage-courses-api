﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using NJsonSchema;
using NSwag.AspNetCore;

using GovUk.Education.ManageCourses.Domain.DatabaseAccess;


namespace GovUk.Education.ManageCourses.Api {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services)
        {
            var connectionString = GetConnectionString();

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<ManageCoursesDbContext>(
                    options => options.UseNpgsql(
                        connectionString, b => b.MigrationsAssembly((typeof(ManageCoursesDbContext).Assembly).ToString())
                        )
                    );

            services.AddScoped<IManageCoursesDbContext>(provider => provider.GetService<ManageCoursesDbContext>());

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ManageCoursesDbContext dbContext)
        {
            dbContext.Database.Migrate();

            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            // Enable the Swagger UI middleware and the Swagger generator
            app.UseSwaggerUi (typeof (Startup).GetTypeInfo ().Assembly, settings => {
                settings.GeneratorSettings.DefaultPropertyNameHandling =
                    PropertyNameHandling.CamelCase;

                settings.PostProcess = document => {
                    document.Info.Version = "v1";
                    document.Info.Title = "Manage courses API";
                    document.Info.Description = "An API for managing course data";
                };

            });

            app.UseMvc();
        }

        private string GetConnectionString()
        {
			var server = Configuration["MANAGE_COURSES_POSTGRESQL_SERVICE_HOST"];
            var port = Configuration["MANAGE_COURSES_POSTGRESQL_SERVICE_PORT"];

            var user = Configuration["PG_USERNAME"];
            var pword = Configuration["PG_PASSWORD"];
            var dbase = Configuration["PG_DATABASE"];

            var sslDefault = "SSL Mode=Prefer;Trust Server Certificate=true";
            var ssl = Configuration["PG_SSL"] ?? sslDefault;

            var connectionString = $"Server={server};Port={port};Database={dbase};User Id={user};Password={pword};{ssl}";
            return connectionString;
        }
    }
}