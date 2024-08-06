using DistributedSystem.API.DependencyInjection.Options;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace DistributedSystem.API.DependencyInjection.Extensions; 
public static class SwaggerExtensions {
    public static void AddSwaggerAPI(this IServiceCollection services) {
        //services.AddSwaggerGen();

        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("test", new OpenApiInfo
            {
                Version = "test",
                Title = "Chat API",
                Description = "Chat API Swagger Surface",
                Contact = new OpenApiContact
                {
                    Name = "Cao Đình Vũ",
                    Email = "caodinhvu00@gmail.com",
                    Url = new Uri("https://www.linkedin.com/in/cao-%C4%91%C3%ACnh-v%C5%A9-205bb230a/")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://github.com/ignaciojvig/ChatAPI/blob/master/LICENSE")
                }

            });

            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

        });





        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    }
    public static void UseSwaggerAPI(this WebApplication app) {

        //app.MapGet("/", () => Results.Redirect("/swagger/index.html")).WithTags(string.Empty);
        app.UseSwagger();
        app.UseSwaggerUI(options => {
            foreach (var version in app.DescribeApiVersions().Select(version => version.GroupName)) {

                options.SwaggerEndpoint($"/swagger/{version}/swagger.json", version);
            }


            options.DisplayRequestDuration();
            options.EnableTryItOutByDefault();
            options.DocExpansion(DocExpansion.None);
        });
    }
}
