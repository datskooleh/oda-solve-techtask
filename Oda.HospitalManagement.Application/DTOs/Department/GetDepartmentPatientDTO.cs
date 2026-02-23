namespace Oda.HospitalManagement.Application.DTOs.Department
{
    public record GetDepartmentPatientDTO(Guid Id, string Name, string AdmissionNumber, DateTime AdmissionDate, Guid DepartmentId);
}
