namespace Oda.HospitalManagement.Application.DTOs.Department
{
    public record UpdateDepartmentDTO(Guid Id, string LongName, string ShortName, byte[] RowVersion);
}
