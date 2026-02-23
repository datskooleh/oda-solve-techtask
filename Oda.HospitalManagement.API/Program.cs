using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Oda.HospitalManagement.API;
using Oda.HospitalManagement.API.Middleware;
using Oda.HospitalManagement.Infrasturcture;
using Oda.HospitalManagement.Persistence.Infrastructure;

using Serilog;

using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

Log.Logger = Extensions.CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddLogger(builder.Configuration);

    // Add services to the container.

    builder.Services.AddControllers();

#if DEBUG
#warning Replace with text/prod context. This is for simplicity and demonstration only.
    builder.Services.AddDbContext<HospitalDbContext>(x => x.UseSqlite("Data Source=hospital.db"));
#else
    throw new Exception("DbContext for testing/production database is not registered");
    //builder.Services.AddDbContext<HospitalDbContext>(x => x.UseSqlServer("Hospital"));
#endif

    builder.Services.AddApplicationServices(builder.Configuration);

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddValidatorsFromAssemblyContaining<Program>();
    builder.Services.AddFluentValidationAutoValidation();

    WebApplication app = builder.Build();

#if DEBUG
#warning This is for simplicity and demonstration only.
    await app.Services.EnsureDbCreationAndDesctuctionAsync();
#endif
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(cfg =>
        {
            cfg.SwaggerEndpoint("/openapi/v1.json", "v1");
            cfg.RoutePrefix = "swagger";
        });
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseMiddleware<TaskCancellationMiddleware>();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
{
    await Log.CloseAndFlushAsync();
}