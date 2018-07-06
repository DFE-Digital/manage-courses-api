using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NSwag.AspNetCore;

namespace GovUk.Education.ManageCourses.Api
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
            var connectionString = GetConnectionString(Configuration);

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<ManageCoursesDbContext>(
                    options => options.UseNpgsql(
                        connectionString, b => b.MigrationsAssembly((typeof(ManageCoursesDbContext).Assembly).ToString())
                    )
                );

            services.AddScoped<IManageCoursesDbContext>(provider => provider.GetService<ManageCoursesDbContext>());

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = BearerTokenDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = BearerTokenDefaults.AuthenticationScheme;
            }).AddBearerToken(options =>
            {
                options.UserinfoEndpoint = Configuration["auth:oidc:userinfo_endpoint"];
            });

            services.AddScoped<IDataService, DataService>();

            services.AddSingleton<IEmailService>(provider => new EmailService(
                Configuration["email:api_key"],
                Configuration["email:template_id"],
                Configuration["email:user"]
            ));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ManageCoursesDbContext dbContext)
        {
            dbContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable the Swagger UI middleware and the Swagger generator
            app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, settings =>
            {
                settings.GeneratorSettings.DefaultPropertyNameHandling =
                    PropertyNameHandling.CamelCase;

                settings.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Manage courses API";
                    document.Info.Description = "An API for managing course data";
                };

            });

            app.UseAuthentication();
            app.UseMvc();
        }

        public static string GetConnectionString(IConfiguration config)
        {
            var server = config["MANAGE_COURSES_POSTGRESQL_SERVICE_HOST"];
            var port = config["MANAGE_COURSES_POSTGRESQL_SERVICE_PORT"];

            var user = config["PG_USERNAME"];
            var pword = config["PG_PASSWORD"];
            var dbase = config["PG_DATABASE"];

            var sslDefault = "SSL Mode=Prefer;Trust Server Certificate=true";
            var ssl = config["PG_SSL"] ?? sslDefault;

            var connectionString = $"Server={server};Port={port};Database={dbase};User Id={user};Password={pword};{ssl}";
            return connectionString;
        }
    }
}
