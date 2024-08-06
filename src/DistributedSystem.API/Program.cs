using Serilog;
using DistributedSystem.Application.DependencyInjection.Extensions;
using DistributedSystem.API.Middleware;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using DistributedSystem.API.DependencyInjection.Extensions;
using Carter;
using Asp.Versioning.ApiExplorer;
using System;
using Swashbuckle.AspNetCore.SwaggerUI;
using DistributedSystem.Persistence.DependencyInjection.Extensions;
using DistributedSystem.Persistence.DependencyInjection.Options;
using DistributedSystem.Infrastructure.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

//Logger
Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging.ClearProviders().AddSerilog();

builder.Host.UseSerilog();

//Add Carter module
builder.Services.AddCarter();

///=> Use in case for ControllerApi difine in DistributedSystem.Presentation
//builder.Services.AddControllers().AddApplicationPart(DistributedSystem.Presentation.AssemblyReference.Assemblu());

//Add Swgger
builder.Services.AddSwaggerGenNewtonsoftSupport()
    .AddFluentValidationRulesToSwagger()
    .AddEndpointsApiExplorer()
    .AddSwaggerAPI();

//Configuration api version
builder.Services.AddApiVersioning(options => options.ReportApiVersions = true)
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

//Configure JWT
builder.Services.AddJwtAuthenticationAPI(builder.Configuration);

builder.Services.AddServicesInfrastructure();
builder.Services.AddRedisInfrastructure(builder.Configuration);
builder.Services.ConfiguretionServiceInfrastructure(builder.Configuration);

//configure MediaR
builder.Services.AddConfigureMediatRApplication();

//configure AutoMapper
builder.Services.AddConfigureAutoMapperApplication();

//Configure masstransit rabbitmq
builder.Services.AddMasstransitRabbitMQInfrastructure(builder.Configuration);

//
builder.Services.AddQuartzInfrastructure();
builder.Services.AddMediatRInfrastructure();

//Configure Options and SQL => Remember mapcarter
builder.Services.AddInterceptorPersistence();
builder.Services.ConfigureSqlServerRetryOptionsPersistence(builder.Configuration.GetSection(nameof(SqlServerRetryOptions)));
builder.Services.AddSqlServerPersistence();
builder.Services.AddRepositoryPersistence();

//Add Middleware => Remember using middleware
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

//Using middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

//app.MapControllers(); //use in case for ControllerApi

app.MapCarter();


//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || builder.Environment.IsStaging())
{
    app.UseSwaggerAPI();
}


try
{
    await app.RunAsync();
    Log.Information("Stopped cleanly");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
    await app.StopAsync();
}
finally
{
    Log.CloseAndFlush();
    await app.DisposeAsync();
}

public partial class Program { }
