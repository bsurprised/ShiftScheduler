using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors;
using Onyx.ShiftScheduler.Api.Exceptions;
using Onyx.ShiftScheduler.Core.Common;
using Onyx.ShiftScheduler.Core.Interfaces;
using Onyx.ShiftScheduler.Infrastructure.Data;
using StructureMap;

namespace Onyx.ShiftScheduler.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static IServiceProvider ServiceProvider { get; private set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("OnyxSchedulerDemo"));
            // or user sqlite, etc -- options.UseSqlite(Configuration.GetConnectionString("SqliteConnection")

            services.AddCors();

            services
                .AddMvcCore() // We use minimum for this api project. If you need the full force of MVC, AddMvc instead
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddControllersAsServices()
                .AddApiExplorer()
                .AddDataAnnotations()
                .AddJsonFormatters()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
                });

            services.AddApiVersioning( /*options => { options.ReportApiVersions = true; }*/);
            services.AddSwagger();

            var container = new Container();
            container.Configure(config =>
            {
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Startup)); // This project
                    _.AssemblyContainingType(typeof(Entity)); // Core project
                    _.AssemblyContainingType(typeof(AppDbContext)); // Infrastructure
                    _.WithDefaultConventions();
                });
                config.For(typeof(IRepository<,>)).Add(typeof(Repository<,>));

                // Populate the container
                config.Populate(services);
            });

            ServiceProvider = container.GetInstance<IServiceProvider>();
            return ServiceProvider;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
            }

            // We use a json object to return errors
            // For debugging in this demo dev version, we also provide the stack
            // It should be removed it production
            app.UseMiddleware<ExceptionHandler>();

            app.UseCors(builder =>
                builder.WithOrigins(Configuration["Origins"].Split(','))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());

            // Swagger Version 1
            app.UseSwaggerUi3WithApiExplorer(settings => // typeof(Startup).GetTypeInfo().Assembly,
            {
                settings.GeneratorSettings.DefaultPropertyNameHandling =
                    PropertyNameHandling.CamelCase;
                settings.GeneratorSettings.DefaultEnumHandling =
                    EnumHandling.String;

                settings.GeneratorSettings.Version = "1";
                settings.SwaggerUiRoute = "/v1/ui";
                settings.SwaggerRoute = "/v1/spec.json";
                settings.GeneratorSettings.OperationProcessors.TryGet<ApiVersionProcessor>().IncludedVersions =
                    new[] {"1"};

                settings.PostProcess = document =>
                {
                    document.Produces = new List<string>
                    {
                        "application/json",
                        "text/json",
                        "text/html",
                        "plain/text",
                        "application/xml"
                    };
                    document.Consumes = document.Produces;

                    document.Info.Version = "v1";
                    document.Info.Title = "Shift Schedule Generator Demo API";
                    document.Info.Description = "Web API for Shift Schedule Generator";
                    document.Info.TermsOfService = "http://localhost:44019";
                    document.Info.Contact = new SwaggerContact
                    {
                        Name = "Onyx",
                        Email = string.Empty,
                        Url = "http://localhost:44019"
                    };
                };
            });

            app.UseSwaggerUi3(s =>
            {
                s.SwaggerRoutes.Add(new SwaggerUi3Route("v1", "/v1/spec.json"));
                s.SwaggerUiRoute = "/ui";
            });

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}