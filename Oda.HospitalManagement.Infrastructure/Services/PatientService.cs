using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Oda.HospitalManagement.Application;
using Oda.HospitalManagement.Application.DTOs;
using Oda.HospitalManagement.Application.DTOs.Patient;
using Oda.HospitalManagement.Application.Interfaces;
using Oda.HospitalManagement.Domain;
using Oda.HospitalManagement.Persistence.Infrastructure;

namespace Oda.HospitalManagement.Infrastructure.Services
{
    internal sealed class PatientService : IPatientService
    {
        private readonly HospitalDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PatientService(HospitalDbContext dbContext, IMapper mapper, ILogger<PatientService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<List<GetPatientDTO>>> FilterAsync(FilteringParametersDTO filtering, CancellationToken cancellationToken)
        {
            var patients = await _dbContext.Patients
                .AsNoTracking()
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Skip((filtering.Page - 1) * filtering.PageSize)
                .Take(filtering.PageSize)
                .Include(x => x.Department)
                .ProjectTo<GetPatientDTO>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new ServiceResult<List<GetPatientDTO>>(data: patients, ResultType.Success);
        }

        public async Task<ServiceResult<GetPatientDTO>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var patient = await _dbContext.Patients
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (patient is null)
                return new ServiceResult<GetPatientDTO>(data: null, ResultType.NotFound);

            return new ServiceResult<GetPatientDTO>(data: _mapper.Map<GetPatientDTO>(patient), ResultType.Success);
        }

        public async Task<ServiceResult<GetPatientDTO>> AddAsync(CreatePatientDTO dto, CancellationToken cancellationToken)
        {
            var firstName = dto.FirstName.Trim();
            var lastName = dto.LastName.Trim();
            var patient = new Patient(firstName, lastName);

            var admissionNumber = dto.AdmissionNumber?.Trim();
            Department? department = null;

            if (!string.IsNullOrWhiteSpace(admissionNumber))
            {
                var hasAdmissionNumber = await _dbContext.Patients
                    .AnyAsync(x => x.AdmissionNumber == admissionNumber, cancellationToken);

                if (hasAdmissionNumber)
                    return new ServiceResult<GetPatientDTO>(message: "Another patient under same admission number is already registered", ResultType.Conflict);

                if (dto.DepartmentId.HasValue && dto.DepartmentId.Value != Guid.Empty)
                {
                    department = await _dbContext.Departments
                        .FirstOrDefaultAsync(x => dto.DepartmentId.Value == x.Id, cancellationToken);

                    if (department == null)
                        return new ServiceResult<GetPatientDTO>("Department should be provided for admissed user", ResultType.Conflict);
                }
            }
            else if (dto.DepartmentId.HasValue)
            {
                return new ServiceResult<GetPatientDTO>("Patient can't be admissed to departemtn as admission number is not provided", ResultType.Conflict);
            }

            if (dto.DepartmentId.HasValue)
                patient.Admit(department!.Id, admissionNumber!, DateTime.UtcNow);

            var entry = await _dbContext.Patients.AddAsync(patient, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ServiceResult<GetPatientDTO>(data: _mapper.Map<GetPatientDTO>(patient), ResultType.Success);
        }

        public async Task<ServiceResult<string>> DischargeAsync(Guid id, CancellationToken cancellationToken)
        {
            var patient = await _dbContext.Patients
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (patient is null)
                return new ServiceResult<string>(message: "Patient not found", ResultType.NotFound);
            else if (patient.Discharged)
                return new ServiceResult<string>(message: "Patient is already discharged", ResultType.Success);

            patient.Discharge(DateTime.UtcNow);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ServiceResult<string>(data: null, ResultType.Success);
        }

        public async Task<ServiceResult<GetPatientDTO>> UpdateAsync(UpdatePatientDTO dto, CancellationToken cancellationToken)
        {
            var patient = await _dbContext.Patients
                .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

            if (patient is null)
                return new ServiceResult<GetPatientDTO>(data: null, ResultType.NotFound);

            patient.Rename(dto.FirstName, dto.LastName);
            patient.RowVersion = dto.RowVersion;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ServiceResult<GetPatientDTO>(data: _mapper.Map<GetPatientDTO>(patient), ResultType.Success);
        }

        public async Task<ServiceResult<GetPatientDTO>> TransferAsync(TransferPatientDTO dto, CancellationToken cancellationToken)
        {
            var patient = await _dbContext.Patients
                .FirstOrDefaultAsync(x => x.Id == dto.PatientId, cancellationToken);

            if (patient == null)
                return new ServiceResult<GetPatientDTO>(message: "Patient is not found", ResultType.NotFound);

            if (!patient.DepartmentId.HasValue)
                return new ServiceResult<GetPatientDTO>(message: "Patient is not admitted to any department", ResultType.Error);

            if (patient.DepartmentId == dto.DepartmentId)
                return new ServiceResult<GetPatientDTO>(message: "Already in the department", ResultType.Success);

            var department = await _dbContext.Departments
                .FirstOrDefaultAsync(x => x.Id == dto.DepartmentId, cancellationToken);

            if (department == null)
                return new ServiceResult<GetPatientDTO>(message: "Department has not been found", ResultType.NotFound);

            patient.Transfer(department);
            patient.RowVersion = dto.RowVersion;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ServiceResult<GetPatientDTO>(data: _mapper.Map<GetPatientDTO>(patient), ResultType.Success);
        }
    }
}