using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Oda.HospitalManagement.Infrastucture.Persistence;

namespace Oda.HospitalManagement.Infrastucture.Persistence.Configuration
{
    //internal sealed class DepartmentsPatientsConfiguration : BaseConfiguration<DepartmentPatients>
    //{
    //    protected override string TableName => "DepartmentPatients";

    //    public override void Configure(EntityTypeBuilder<DepartmentPatients> builder)
    //    {
    //        base.Configure(builder);

    //        builder.HasIndex(x => x.DepartmentId);

    //        builder.HasIndex(x => x.PatientId)
    //            .IsUnique();

    //        builder.Property(x => x.DepartmentId)
    //            .IsRequired();

    //        builder.Property(x => x.PatientId)
    //            .IsRequired();
    //    }
    //}
}
