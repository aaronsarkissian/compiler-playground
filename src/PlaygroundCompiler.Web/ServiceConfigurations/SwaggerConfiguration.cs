using Microsoft.OpenApi.Models;
using System.Reflection;

namespace PlaygroundService.ServiceConfigurations
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            bool enabled = Convert.ToBoolean(configuration["Swagger:Enabled"]);
            if (enabled)
            {

                services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter 'Bearer' [space] and then your token in the text input below.Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });

                var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                c.IncludeXmlComments(xmlPath);

            });
            }
            return services;
        }

        public static IApplicationBuilder UseSLSwagger(this IApplicationBuilder app, IConfiguration configuration)
        {
            bool enabled = Convert.ToBoolean(configuration["Swagger:Enabled"]);
            if (enabled)
            {
                string jsonRoutePrefix = configuration["Swagger:JSONRoutePrefix"];
                string swaggerBasePath = configuration["Swagger:ApiBasePath"];
                string swaggreApiScheme = configuration["Swagger:ApiBaseScheme"];

                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "swagger/{documentName}/swagger.json";
                    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    {
                        swaggerDoc.Servers = new List<OpenApiServer>
                        {
                        new OpenApiServer
                        {
                            Url = $"{swaggreApiScheme}://{httpReq.Host.Value}{swaggerBasePath}"
                        }
                        };
                    });
                });
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"{jsonRoutePrefix}/swagger/v1/swagger.json", "API V1");
                    c.RoutePrefix = "swagger";
                });
            }

            return app;
        }
    }
}
