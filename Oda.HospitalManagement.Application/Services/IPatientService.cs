using Oda.HospitalManagement.Application.DTOs;
using Oda.HospitalManagement.Application.DTOs.Patient;

namespace Oda.HospitalManagement.Application.Interfaces
{
    public interface IPatientService
    {
        Task<ServiceResult<string>> DischargeAsync(Guid id, CancellationToken cancellationToken);

        //Task<ServiceResult<string>> AssignAsync(AdmitPatientDTO dto, CancellationToken cancellationToken);

        Task<ServiceResult<List<GetPatientDTO>>> FilterAsync(FilteringParametersDTO filtering, CancellationToken cancellationToken);

        Task<ServiceResult<GetPatientDTO>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<ServiceResult<GetPatientDTO>> AddAsync(CreatePatientDTO dto, CancellationToken cancellationToken);

        Task<ServiceResult<GetPatientDTO>> UpdateAsync(UpdatePatientDTO dto, CancellationToken cancellationToken);
     
        Task<ServiceResult<GetPatientDTO>> TransferAsync(TransferPatientDTO dto, CancellationToken cancellationToken);
    }
}