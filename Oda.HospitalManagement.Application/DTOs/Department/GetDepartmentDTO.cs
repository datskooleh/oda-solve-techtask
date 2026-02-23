namespace Oda.HospitalManagement.Application.DTOs.Department
{
    public record GetDepartmentDTO(Guid Id, string ShortName, string LongName, byte[] RowVersion)
    {
        public string Name => $"{ShortName}, ({LongName})";
    }
}
