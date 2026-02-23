using Oda.HospitalManagement.Application.DTOs;
using Oda.HospitalManagement.Application.DTOs.Department;

namespace Oda.HospitalManagement.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<ServiceResult<List<GetDepartmentDTO>>> FilterAsync(FilteringParametersDTO filtering, CancellationToken cancellationToken);

        Task<ServiceResult<string>> AssignAsync(AdmitPatientDTO dto, CancellationToken cancellationToken);

        Task<ServiceResult<GetDepartmentDTO>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<ServiceResult<List<GetDepartmentPatientDTO>>> GetOccupationAsync(Guid departmentId, DateOnly? submissionDate, FilteringParametersDTO filtering, CancellationToken cancellationToken);

        Task<ServiceResult<GetDepartmentDTO>> AddDepartmentAsync(CreateDepartmentDTO dto, CancellationToken cancellationToken);

        Task<ServiceResult<string>> DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task<ServiceResult<GetDepartmentDTO>> UpdateAsync(UpdateDepartmentDTO dto, CancellationToken cancellationToken);
    }
}