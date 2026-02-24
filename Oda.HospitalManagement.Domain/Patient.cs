using Oda.HospitalManagement.Domain.Exceptions;
using Oda.HospitalManagement.Domain.Validators;

namespace Oda.HospitalManagement.Domain
{
    public sealed class Patient : BaseEntity
    {
        public Patient(string firstName, string lastName)
            : base() => Rename(firstName, lastName);

        public Patient(Guid id, string firstName, string lastName)
            : base(id, null) => Rename(firstName, lastName);

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public bool Discharged { get; private set; } = true;

        public string? AdmissionNumber { get; private set; }

        public DateTime? AdmissionDate { get; private set; }

        public Guid? DepartmentId { get; private set; }

        public Department? Department { get; private set; }

        public DateTime? DischargeDate { get; private set; }

        public string Name => $"{LastName}, {FirstName}";

        public void Discharge(DateTime dischargeDate)
        {
            PatientValidator.EnsureDischargeIsValid(Discharged, dischargeDate, AdmissionDate);
            AdmissionNumber = null;
            DepartmentId = null;
            Department = null;
            DischargeDate = dischargeDate;
            Discharged = true;

            UpdatedAt = DateTime.UtcNow;
        }

        public void Transfer(Department department)
        {
            AdmissionNumber.EnsureAdmissionNumberIsValid();

            if (department == null || department.Id == Guid.Empty)
                throw new AdmissionDomainException("Department does not exist");

            DepartmentId = department.Id;

            UpdatedAt = DateTime.UtcNow;
        }

        public void Admit(Guid departmentId, string admissionNumber, DateTime? admissionDate)
        {
            admissionNumber = admissionNumber.Trim();

            admissionNumber.EnsureAdmissionNumberIsValid();

            if (departmentId == Guid.Empty)
                throw new AdmissionDomainException("Department does not exist");

            AdmissionDate = admissionDate ?? DateTime.UtcNow;
            AdmissionNumber = admissionNumber;
            DepartmentId = departmentId;
            Discharged = false;
            DischargeDate = null;

            UpdatedAt = DateTime.UtcNow;
        }

        public void Rename(string firstName, string lastName)
        {
            firstName = firstName.Trim();
            lastName = lastName.Trim();

            firstName.EnsureValidName(nameof(FirstName), PatientValidator.FirstNameMaxLength);
            lastName.EnsureValidName(nameof(LastName), PatientValidator.LastNameMaxLength);

            FirstName = firstName;
            LastName = lastName;

            UpdatedAt = DateTime.UtcNow;
        }
    }
}