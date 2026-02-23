namespace Oda.HospitalManagement.Application.DTOs.Department
{
    public record AdmitPatientDTO(Guid Id, Guid DepartmentId, string AdmissionNumber, DateTime? AdmissionDate, byte[] RowVersion);
}