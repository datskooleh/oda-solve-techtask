namespace Oda.HospitalManagement.Application.DTOs.Patient
{
    public record UpdatePatientDTO(Guid Id, string FirstName, string LastName, byte[] RowVersion);
}