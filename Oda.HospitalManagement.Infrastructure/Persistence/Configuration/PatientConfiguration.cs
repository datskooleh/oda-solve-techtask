using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Oda.HospitalManagement.Domain;
using Oda.HospitalManagement.Domain.Validators;

namespace Oda.HospitalManagement.Infrastucture.Persistence.Configuration
{
    internal sealed class PatientConfiguration : BaseConfiguration<Patient>
    {
        protected override string TableName => "Patients";

        public override void Configure(EntityTypeBuilder<Patient> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(PatientValidator.FirstNameMaxLength);

            builder.Property(x => x.LastName)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(PatientValidator.LastNameMaxLength);

            builder.Property(x => x.AdmissionNumber)
                .HasMaxLength(PatientValidator.AdmissionNumberMaxLength);

            builder.Property(x => x.AdmissionDate)
                .IsRequired();

            builder.Property(x => x.Discharged)
                .IsRequired();

            builder.Ignore(x => x.Name);
        }
    }
}
