using System.Reflection;

using Microsoft.EntityFrameworkCore;

using Oda.HospitalManagement.Domain;

namespace Oda.HospitalManagement.Persistence.Infrastructure
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Patient> Patients { get; set; }
    }
}