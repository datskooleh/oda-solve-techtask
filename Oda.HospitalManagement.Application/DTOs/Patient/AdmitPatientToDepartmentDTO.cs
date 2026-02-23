namespace Oda.HospitalManagement.Application.DTOs.Patient
{
    public record AdmitPatientToDepartmentDTO(Guid Id, Guid DepartmentId, string? AdmissionNumber, DateTime? AdmissionDate, byte[] RowVersion);
}