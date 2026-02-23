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
            var admissionNumber = dto.AdmissionNumber?.Trim();

            if (!string.IsNullOrWhiteSpace(admissionNumber))
            {
                var hasAdmissionNumber = await _dbContext.Patients
                    .AnyAsync(x => x.AdmissionNumber == admissionNumber, cancellationToken);

                if (hasAdmissionNumber)
                    return new ServiceResult<GetPatientDTO>(message: "Another patient under same admission number is already registered", ResultType.Conflict);
            }

            Department? department = null;
            if (dto.DepartmentId.HasValue && dto.DepartmentId.Value != Guid.Empty)
            {
                department = await _dbContext.Departments
                    .FirstOrDefaultAsync(x => dto.DepartmentId.Value == x.Id, cancellationToken);
            }

            var firstName = dto.FirstName.Trim();
            var lastName = dto.LastName.Trim();

            var patient = new Patient(firstName, lastName);
            if (string.IsNullOrWhiteSpace(admissionNumber) && department != null)
                return new ServiceResult<GetPatientDTO>(message: "", ResultType.Conflict);

            patient.Admit(department, admissionNumber, DateTime.UtcNow);

            var entry = await _dbContext.Patients.AddAsync(patient, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ServiceResult<GetPatientDTO>(data: _mapper.Map<GetPatientDTO>(patient), ResultType.Success);
        }

        //public async Task<ServiceResult<string>> AssignAsync(AdmitPatientDTO dto, CancellationToken cancellationToken)
        //{
        //    var admissionNumber = dto.AdmissionNumber?.Trim();

        //    if (!string.IsNullOrWhiteSpace(admissionNumber))
        //    {
        //        var hasAdmissionNumber = await _dbContext.Patients
        //            .AnyAsync(x => x.AdmissionNumber == admissionNumber && x.Id != dto.Id, cancellationToken);

        //        if (hasAdmissionNumber)
        //            return new ServiceResult<string>(message: "Another patient under same admission number is already registered", ResultType.Conflict);
        //    }

        //    var department = await _dbContext.Departments
        //        .FirstOrDefaultAsync(x => x.Id == dto.DepartmentId, cancellationToken);

        //    if (department is null)
        //        return new ServiceResult<string>(message: "Department does not exist", ResultType.NotFound);

        //    var patient = await _dbContext.Patients
        //        .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

        //    if (patient is null)
        //        return new ServiceResult<string>(message: "Patient not found", ResultType.NotFound);
        //    else if (patient.AdmissionNumber == admissionNumber)
        //        return new ServiceResult<string>(message: "Patient is already amissed under different number", ResultType.Conflict);
        //    else if (patient.DepartmentId == dto.DepartmentId)
        //        return new ServiceResult<string>("Already in same department", ResultType.Success);

        //    if (string.IsNullOrWhiteSpace(admissionNumber))
        //        patient.Transfer(department);
        //    else
        //        patient.Admit(department, admissionNumber, dto.AdmissionDate);

        //    patient.RowVersion = dto.RowVersion;
        //    await _dbContext.SaveChangesAsync(cancellationToken);

        //    return new ServiceResult<string>(data: null, ResultType.Success);
        //}

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
            var patient = await _dbContext.Patients.FirstOrDefaultAsync(x => x.Id == dto.PatientId, cancellationToken);

            if (patient == null)
                return new ServiceResult<GetPatientDTO>(message: "Patient is not found", ResultType.NotFound);

            var department = await _dbContext.Departments.FirstOrDefaultAsync(x => x.Id == dto.DepartmentId, cancellationToken);

            if (department == null)
                return new ServiceResult<GetPatientDTO>(message: "Department has not been found", ResultType.NotFound);

            patient.Transfer(department);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ServiceResult<GetPatientDTO>(data: _mapper.Map<GetPatientDTO>(patient), ResultType.Success);
        }
    }
}