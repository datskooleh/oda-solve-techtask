namespace Oda.HospitalManagement.Application.DTOs.Patient
{
    public record TransferPatientDTO(Guid PatientId, Guid DepartmentId, byte[] RowVersion);
}