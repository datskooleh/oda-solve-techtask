using System.ComponentModel.DataAnnotations;

namespace Oda.HospitalManagement.Application.DTOs
{
    public record FilteringParametersDTO(int Page = 1, int PageSize = 10, string? Filter = null);
}
