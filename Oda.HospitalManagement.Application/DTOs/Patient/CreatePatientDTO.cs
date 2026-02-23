namespace Oda.HospitalManagement.Application.DTOs.Patient
{
    public record CreatePatientDTO(string FirstName, string LastName, string? AdmissionNumber, Guid? DepartmentId);
}