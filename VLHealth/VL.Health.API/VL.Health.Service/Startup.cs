using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Reflection;
using VL.Health.IoC;
using Microsoft.OpenApi.Models;
using VL.Health.Service.Swagger;
using VL.Libraries.Client.Interceptor;
using System.Collections.Generic;
using VL.Health.Service.CustomExceptions;
using VL.Security.Libraries.IoC;
using VL.Libraries.TenantDataAccess;
using VL.Libraries.Client;
using VL.Audit.Client.IoC;
using VL.Health.Infrastructure.Interceptors;

namespace VL.Health.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string CorsPolicy = "CorsPolicy";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // The following line enables Application Insights telemetry collection.           
            services.AddApplicationInsightsTelemetry();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllers();
            services.AddMvc().AddNewtonsoftJson();
            services.AddMvcCore();

            services.ConfigureIoC();
            services.ConfigureSecurityLibrariesIoC();

            services.ConfigureTenantDataAccess(Configuration);
            services.ConfigureServiceGateway();
            services.ConfigureAudit();

            services.AddSwaggerGen(x =>
            {
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",                    
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
                x.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = $"{this.GetType().Assembly.GetCustomAttribute<System.Reflection.AssemblyProductAttribute>().Product}",
                    Version = Environment.GetEnvironmentVariable("BuildId"),
                    Description = "Health"
                });
            });

            services.AddLogging(); 

            services.AddHttpContextAccessor();
            services.TryAddTransient<AuthorizationHeaderHandler>();
            services.TryAddTransient<WebApiTenantHeaderFixerHandler>();
            services.AddHttpClient("AuthorizedClient")
                .AddHttpMessageHandler<AuthorizationHeaderHandler>()
                .AddHttpMessageHandler<WebApiTenantHeaderFixerHandler>();

            services.AddCors(opt =>
            {
                opt.AddPolicy(CorsPolicy,
                c => c.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<CustomExceptionMiddleware>();


            app.UseCors(CorsPolicy);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);
            app.UseSwagger(option => { option.RouteTemplate = swaggerOptions.RouteTemplate; });
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint("/swagger/v1/swagger.json", "Security API V1");
            });
        }
    }
}
