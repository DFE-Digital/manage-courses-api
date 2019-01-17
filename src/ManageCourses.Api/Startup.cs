using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using GovUk.Education.ManageCourses.Api.ActionFilters;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Middleware;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.AccessRequests;
using GovUk.Education.ManageCourses.Api.Services.Data;
using GovUk.Education.ManageCourses.Api.Services.Email;
using GovUk.Education.ManageCourses.Api.Services.Email.Config;
using GovUk.Education.ManageCourses.Api.Services.Invites;
using GovUk.Education.ManageCourses.Api.Services.Publish;
using GovUk.Education.ManageCourses.Api.Services.Users;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.SearchAndCompare.Domain.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;
using Serilog;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;

namespace GovUk.Education.ManageCourses.Api
{
    public class Startup
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public Startup(IConfiguration configuration, ILoggerFactory logFactory)
        {
            _logger = logFactory.CreateLogger<Startup>();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            var mcConfig = new McConfig(Configuration);
            mcConfig.Validate();
            var connectionString = mcConfig.BuildConnectionString();

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<ManageCoursesDbContext>(
                    options =>
                    {
                        const int maxRetryCount = 3;
                        const int maxRetryDelaySeconds = 5;

                        var postgresErrorCodesToConsiderTransient = new List<string>(); // ref: https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL/blob/16c8d07368cb92e10010b646098b562ecd5815d6/src/EFCore.PG/NpgsqlRetryingExecutionStrategy.cs#L99

                        // Note that the retry will only retry for TimeoutExceptions and transient postgres exceptions. ref: https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL/blob/8e97e4195b197ae3d16763704352acfffa95c73f/src/EFCore.PG/Storage/Internal/NpgsqlTransientExceptionDetector.cs#L12
                        options.UseNpgsql(connectionString,
                            b => b.MigrationsAssembly((typeof(ManageCoursesDbContext).Assembly).ToString())
                                .EnableRetryOnFailure(maxRetryCount, TimeSpan.FromSeconds(maxRetryDelaySeconds), postgresErrorCodesToConsiderTransient));
                    });

            services.AddScoped<IManageCoursesDbContext>(provider => provider.GetService<ManageCoursesDbContext>());

            // No default auth method has been set here because each action must explictly be decorated with
            // either BearerTokenAuthAttribute or ApiTokenAuthAttribute to avoid unwanted calls to the wrong one.
            services.AddAuthentication()
                .AddBearerToken(options =>
                {
                    options.UserinfoEndpoint = mcConfig.SignInUserInfoEndpoint;
                })
                .AddBearerTokenApiKey(options =>
                {
                    options.ApiKey = mcConfig.ApiKey;
                });

            services.AddScoped<ISearchAndCompareService, SearchAndCompareService>();
            services.AddScoped<ICourseMapper, CourseMapper>();
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IInviteService, InviteService>();
            services.AddScoped<IEnrichmentService, EnrichmentService>();
            services.AddScoped<IWelcomeTemplateEmailConfig, WelcomeTemplateEmailConfig>();
            services.AddScoped<IWelcomeEmailService, WelcomeEmailService>();
            services.AddScoped<IClock, Clock>();

            services.AddScoped<IInviteTemplateEmailConfig, InviteTemplateEmailConfig>();
            services.AddScoped<IInviteEmailService, InviteEmailService>();

            services.AddScoped<IAccessRequestService>(provider =>
            {
                return new AccessRequestService(provider.GetService<IManageCoursesDbContext>(),
                 new EmailServiceFactory(mcConfig.EmailApiKey)
                 .MakeAccessRequestEmailService(
                     mcConfig.EmailTemplateId,
                     mcConfig.EmailUser
                 ));
            });

            services.AddScoped<ISearchAndCompareApi>(provider =>
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", mcConfig.SearchAndCompareApiKey);
                return new SearchAndCompareApi(httpClient, mcConfig.SearchAndCompareApiUrl);
            });
            services.AddScoped<INotificationClientWrapper, NotificationClientWrapper>();

            services.AddMvc(options =>
                options.Filters.Add(typeof(AcceptTermsFilter))
            ).AddJsonOptions(x =>
            {
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
                x.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ManageCoursesDbContext dbContext)
        {
            Migrate(dbContext);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.SetSecurityHeaders();
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

        /// <summary>
        /// Migrate with inifinte retry.
        /// </summary>
        /// <param name="dbContext"></param>
        private void Migrate(ManageCoursesDbContext dbContext)
        {
            // If the migration fails and throws then the app ends up in a broken state so don't let that happen.
            // If the migrations failed and the exception was swallowed then the code could make assumptions that result in corrupt data so don't let execution continue till this has worked.
            int migrationAttempt = 1;
            while (true)
            {
                try
                {
                    _logger.LogInformation($"Applying EF migrations. Attempt {migrationAttempt} of ∞");
                    dbContext.Database.Migrate();
                    _logger.LogInformation($"Applying EF migrations succeeded. Attempt {migrationAttempt} of ∞");
                    break; // success!
                }
                catch (Exception ex)
                {
                    const int maxDelayMs = 60 * 1000;
                    int delayMs = 1000 * migrationAttempt;
                    if (delayMs > maxDelayMs)
                    {
                        delayMs = maxDelayMs;
                    }
                    // exception included in message string because app insights isn't showing the messages and kudo log stream only shows the message string.
                    _logger.LogError($"Failed to apply EF migrations. Attempt {migrationAttempt} of ∞. Waiting for {delayMs}ms before trying again.\n{ex}", ex);
                    new TelemetryClient().TrackException(ex);
                    Thread.Sleep(delayMs);
                    migrationAttempt++;
                }
            }
        }
    }
}
