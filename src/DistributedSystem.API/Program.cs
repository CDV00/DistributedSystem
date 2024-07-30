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

builder.Services.AddInfrastructureServices();
builder.Services.AddRedisService(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddConfigureMediatR();
builder.Services.AddConfigureAutoMapper();

//Configure Options and SQL => Remember mapcarter
builder.Services.AddInterceptorDbContext();
builder.Services.ConfigureSqlServerRetryOptions(builder.Configuration.GetSection(nameof(SqlServerRetryOptions)));
builder.Services.AddSqlServerConfiguration();
builder.Services.AddRepositoryBaseConfiguration();


//Add Middleware => Remember using middleware
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

//Add Carter module
builder.Services.AddCarter();

//Add Swgger
builder.Services.AddSwaggerGenNewtonsoftSupport()
    .AddFluentValidationRulesToSwagger()
    .AddEndpointsApiExplorer()
    .AddSwagger();

builder.Services.AddApiVersioning(options => options.ReportApiVersions = true)
    .AddApiExplorer(options => {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });



var app = builder.Build();

//Using middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapCarter();




//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || builder.Environment.IsStaging()) {
    app.ConfigureSwagger();
}


try {
    await app.RunAsync();
    Log.Information("Stopped cleanly");
}
catch(Exception ex) {
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
    await app.StopAsync();
} finally {
    Log.CloseAndFlush();
    await app.DisposeAsync();
}

public partial class Program { }
