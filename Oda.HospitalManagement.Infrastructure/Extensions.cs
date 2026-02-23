using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Oda.HospitalManagement.Application.Interfaces;
using Oda.HospitalManagement.Infrastructure.Services;

using Serilog;

namespace Oda.HospitalManagement.Infrasturcture
{
    public static class Extensions
    {
        public static ILogger CreateLogger()
        {
            return new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        public static void AddLogger(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddSerilog((sp, config) =>
                {
                    config.ReadFrom.Configuration(configuration);
                    config.ReadFrom.Services(sp);
                    config.Enrich.FromLogContext();
                });
        }

        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(cfg =>
            {
                if (!string.IsNullOrWhiteSpace(configuration["AutomapperLicense"]))
                    cfg.LicenseKey = configuration["AutomapperLicense"];
            }, Assembly.GetExecutingAssembly());

            services.AddTransient<IDepartmentService, DepartmentService>();
            services.AddTransient<IPatientService, PatientService>();
        }
    }
}
