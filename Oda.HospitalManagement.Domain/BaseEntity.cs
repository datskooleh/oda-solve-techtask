namespace Oda.HospitalManagement.Domain
{
    public abstract class BaseEntity
    {
        protected BaseEntity() { }

        protected BaseEntity(Guid id, DateTime? createdAt)
        {
            Id = id;

            if (createdAt.HasValue)
                CreatedAt = createdAt.Value;
        }

        public Guid Id { get; init; }

        public DateTime CreatedAt { get; }

        public DateTime? UpdatedAt { get; protected set; }

        public byte[] RowVersion { get; set; }
    }
}
