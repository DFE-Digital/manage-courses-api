using System.Reflection;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.AccessRequests;
using GovUk.Education.ManageCourses.Api.Services.Data;
using GovUk.Education.ManageCourses.Api.Services.Email;
using GovUk.Education.ManageCourses.Api.Services.Email.Config;
using GovUk.Education.ManageCourses.Api.Services.Invites;
using GovUk.Education.ManageCourses.Api.Services.Users;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;

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
            var mcConfig = new McConfig(Configuration);
            var connectionString = mcConfig.BuildConnectionString();

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<ManageCoursesDbContext>(
                    options =>
                    {
                        options.UseNpgsql(connectionString,
                            b => b.MigrationsAssembly((typeof(ManageCoursesDbContext).Assembly).ToString())
                        );
                    });

            services.AddScoped<IManageCoursesDbContext>(provider => provider.GetService<ManageCoursesDbContext>());

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = BearerTokenDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = BearerTokenDefaults.AuthenticationScheme;
            }).AddBearerToken(options =>
            {
                options.UserinfoEndpoint = Configuration["auth:oidc:userinfo_endpoint"];
            }).AddBearerTokenApiKey(options =>
            {
                options.ApiKey = Configuration["api:key"];
            });

            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IInviteService, InviteService>();
            services.AddScoped<IWelcomeTemplateEmailConfig, WelcomeTemplateEmailConfig>();
            services.AddScoped<IWelcomeEmailService, WelcomeEmailService>();
            services.AddScoped<IClock, Clock>();

            services.AddScoped<IInviteTemplateEmailConfig, InviteTemplateEmailConfig>();
            services.AddScoped<IInviteEmailService, InviteEmailService>();

            services.AddScoped<IAccessRequestService>(provider =>
            {
                return new AccessRequestService(provider.GetService<IManageCoursesDbContext>(),
                 new EmailServiceFactory(Configuration["email:api_key"])
                 .MakeAccessRequestEmailService(
                     Configuration["email:template_id"],
                     Configuration["email:user"]
                 ));
            });

            services.AddScoped<INotificationClientWrapper, NotificationClientWrapper>();
            services.AddScoped<IDataHelper, UserDataHelper>();

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
            app.UseSwaggerUi3(typeof(Startup).GetTypeInfo().Assembly, settings =>
            {
                settings.GeneratorSettings.DefaultPropertyNameHandling =
                    PropertyNameHandling.CamelCase;

                settings.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Manage courses API";
                    document.Info.Description = "An API for managing course data";
                };
                settings.GeneratorSettings.DocumentProcessors.Add(new SecurityDefinitionAppender(BearerTokenDefaults.AuthenticationScheme, new SwaggerSecurityScheme
                {
                    Type = SwaggerSecuritySchemeType.ApiKey,
                    Description = "In order to interactive with the api please input `Bearer {code}`",
                    In = SwaggerSecurityApiKeyLocation.Header,
                    Name = "Authorization"
                }));

                settings.GeneratorSettings.OperationProcessors.Add(new OperationSecurityScopeProcessor(BearerTokenDefaults.AuthenticationScheme));
            });

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
