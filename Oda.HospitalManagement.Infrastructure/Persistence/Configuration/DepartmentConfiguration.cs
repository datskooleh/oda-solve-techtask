using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Oda.HospitalManagement.Domain;
using Oda.HospitalManagement.Domain.Validators;

namespace Oda.HospitalManagement.Infrastucture.Persistence.Configuration
{
    internal sealed class DepartmentConfiguration : BaseConfiguration<Department>
    {
        protected override string TableName => "Departments";

        public override void Configure(EntityTypeBuilder<Department> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.ShortName)
                .IsRequired()
                .HasMaxLength(DepartmentValidator.ShortNameMaxLength);

            builder.Property(x => x.LongName)
                .IsRequired()
                .HasMaxLength(DepartmentValidator.LongNameMaxLength);

            builder.HasIndex(x => x.ShortName)
                .IsUnique();

            builder.HasMany(x => x.Patients)
                .WithOne(x => x.Department)
                .HasForeignKey(x => x.DepartmentId);
        }
    }
}
