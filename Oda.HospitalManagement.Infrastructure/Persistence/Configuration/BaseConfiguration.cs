using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Oda.HospitalManagement.Domain;

namespace Oda.HospitalManagement.Infrastucture.Persistence.Configuration
{
    internal abstract class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        protected abstract string TableName { get; }

        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

#if DEBUG
#warning This code is used for the purpose of showcasing for technical task only.
            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            //this value will not work in SQLite as per DB limitation
            builder.Property(x => x.RowVersion)
                .HasDefaultValueSql("randomblob(8)")
                .ValueGeneratedOnAddOrUpdate()
                .IsRowVersion()
                .IsConcurrencyToken();
#else
            builder.Property(x => x.DateCreated)
                .IsRequired()
                .HasDefaultValue("SYSUTCDATETIME()");

            builder.Property(x => x.RowVersion)
                .IsRowVersion()
                .ValueGeneratedOnUpdate();
#endif
        }
    }
}
