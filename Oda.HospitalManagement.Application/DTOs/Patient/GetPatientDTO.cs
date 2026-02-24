namespace Oda.HospitalManagement.Application.DTOs.Patient
{
    public record GetPatientDTO(Guid Id, string FirstName, string LastName, bool Discharged, string? AdmissionNumber, DateTime? AdmissionDate, string? DepartmentName, Guid? DepartmentId, DateTime? DischargeDate, byte[] RowVersion)
    {
        public string Name => $"{LastName}, {FirstName}";
    }
}