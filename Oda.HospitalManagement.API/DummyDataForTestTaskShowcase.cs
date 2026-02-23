using Microsoft.EntityFrameworkCore;

using Oda.HospitalManagement.Persistence.Infrastructure;

namespace Oda.HospitalManagement.API
{

#if DEBUG
#warning This should not be enabled in DEV+ environments. keep only to local
#warning Unreference DOMAIN and INFRASTRUCTURE from project after this one
    //only for demonstation purposes
    public static class DummyDataForTestTaskShowcase
    {
        public static async Task EnsureDbCreationAndDesctuctionAsync(this IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Ensuring database is created");

                var dbContext = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
                await dbContext.Database.EnsureDeletedAsync();
                await dbContext.Database.EnsureCreatedAsync();

                await dbContext.AddDummyRecordsAsync();

                logger.LogInformation("Registering database drop on application down");
                var lifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
                lifetime.ApplicationStopping.Register(() =>
                {
                    using var scope = provider.CreateScope();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    logger.LogInformation("Dropping database");
                    var dbContext = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
                    dbContext.Database.EnsureDeleted();
                    logger.LogInformation("Database is dropped");
                });
            }
        }

        public static async Task AddDummyRecordsAsync(this HospitalDbContext dbContext)
        {
            var dummyRowVersion = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());

            var departments = new Oda.HospitalManagement.Domain.Department[]
            {
        GetDummyDepartment("IM", "Innere Medizin"),
        GetDummyDepartment("ITS", "Intensivstation"),
        GetDummyDepartment("CHIR", "Allgemeine Chirurgie"),
        GetDummyDepartment("IMC", "Intermediate Care"),
        GetDummyDepartment("AWR", "Aufwachraum"),
        GetDummyDepartment("GASTRO", "Gastroenterologie"),
        GetDummyDepartment("GCH", "Gefäßchirurgie"),
            };

            await dbContext.Departments.AddRangeAsync(departments);

            var addedDepartments = dbContext.ChangeTracker.Entries<Oda.HospitalManagement.Domain.Department>()
                .Where(x => x.State == EntityState.Added)
                .Select(x => x.Entity);

            var rand = new Random();
            var patients = new Oda.HospitalManagement.Domain.Patient[]
            {
        GetDummyAdmissionedPatient("Leon", "Müller", "4821/92", DateTime.UtcNow.AddDays(-rand.Next(15)), addedDepartments.First().Id),
        GetDummyAdmissionedPatient("Sophie", "Schmidt", "1503/44", DateTime.UtcNow.AddDays(-rand.Next(15)), addedDepartments.Skip(1).First().Id),
        GetDummyAdmissionedPatient("Noah", "Schneider", "7291/18", DateTime.UtcNow.AddDays(-rand.Next(15)), addedDepartments.Skip(2).First().Id),
        GetDummyAdmissionedPatient("Mia", "Fischer  ", "3367/56", DateTime.UtcNow.AddDays(-rand.Next(15)), addedDepartments.Skip(3).First().Id),
        GetDummyAdmissionedPatient("Paul", "Weber", "9012/31", DateTime.UtcNow.AddDays(-rand.Next(15)), addedDepartments.Skip(3).First().Id),
        GetDummyAdmissionedPatient("Emma", "Meyer", "2485/77", DateTime.UtcNow.AddDays(-rand.Next(15)), addedDepartments.Skip(4).First().Id),
        GetDummyAdmissionedPatient("Elias", "Wagner", "6134/20", DateTime.UtcNow.AddDays(-rand.Next(15)), addedDepartments.Skip(4).First().Id),
        GetDummyAdmissionedPatient("Hanna", "Becker", "8840/63", DateTime.UtcNow.AddDays(-rand.Next(15)), addedDepartments.Skip(4).First().Id),
        GetDummyAdmissionedPatient("Marie", "Hoffmann", "5219/85", DateTime.UtcNow.AddDays(-rand.Next(15)), addedDepartments.Skip(5).First().Id),
        GetDummyAdmissionedPatient("Luis", "Schäfer", "1076/12", DateTime.UtcNow.AddDays(-rand.Next(15)), addedDepartments.First().Id),
        GetDummyAdmissionedPatient("Emilia", "Koch", "3958/49", DateTime.UtcNow.AddDays(-rand.Next(15)), addedDepartments.First().Id),
        GetDummyDischargedPatient("Jonas", "Schulz", DateTime.UtcNow.AddDays(-5)),
            };

            await dbContext.Patients.AddRangeAsync(patients);

            await dbContext.SaveChangesAsync();

            Oda.HospitalManagement.Domain.Department GetDummyDepartment(string shortName, string longName)
            {
                var dep = new Oda.HospitalManagement.Domain.Department(Guid.NewGuid());
                dep.Rename(shortName, longName);
                return dep;
            }

            Oda.HospitalManagement.Domain.Patient GetDummyAdmissionedPatient(string firstName, string lastName, string admissionNumber, DateTime admissionDate, Guid departmentId)
            {
                var patient = new Oda.HospitalManagement.Domain.Patient(Guid.NewGuid(), firstName, lastName);
                patient.Rename(firstName, lastName);
                patient.Admit(admissionNumber, admissionDate, departmentId);

                return patient;
            }

            Oda.HospitalManagement.Domain.Patient GetDummyDischargedPatient(string firstName, string lastName, DateTime dischargeDate)
            {
                var patient = new Oda.HospitalManagement.Domain.Patient(Guid.NewGuid(), firstName, lastName);
                patient.Rename(firstName, lastName);

                return patient;
            }
        }
    }
#endif
}