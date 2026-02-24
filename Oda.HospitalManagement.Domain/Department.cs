using Oda.HospitalManagement.Domain.Validators;

namespace Oda.HospitalManagement.Domain
{
    public sealed class Department : BaseEntity
    {
        public Department()
            : base()
        { }

        public Department(Guid id)
            : base(id, null)
        { }

        public string ShortName { get; private set; }

        public string LongName { get; private set; }


        private readonly List<Patient> _patients = [];

        public IReadOnlyCollection<Patient> Patients => _patients;

        public void Rename(string shortName, string longName)
        {
            shortName = shortName.Trim().ToUpper();
            longName = longName.Trim();

            shortName.EnsureValidName(nameof(ShortName), DepartmentValidator.ShortNameMaxLength);
            longName.EnsureValidName(nameof(LongName), DepartmentValidator.LongNameMaxLength);

            ShortName = shortName;
            LongName = longName;
        }

        public string Name => $"{ShortName} ({LongName})";
    }
}