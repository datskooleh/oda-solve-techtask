using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Oda.HospitalManagement.Application;
using Oda.HospitalManagement.Application.DTOs;
using Oda.HospitalManagement.Application.DTOs.Department;
using Oda.HospitalManagement.Application.Interfaces;
using Oda.HospitalManagement.Domain;
using Oda.HospitalManagement.Persistence.Infrastructure;

namespace Oda.HospitalManagement.Infrastructure.Services
{
#warning in this case consider using CQRS since service becomes bloated
    internal sealed class DepartmentService : IDepartmentService
    {
#warning create Middleware for processing cancellation instead of catching it everywhere.
#warning create Middleware for processing 500 errors in one place and return BadRequest
        private static readonly ServiceResult<GetDepartmentDTO> _cancellationTaskResult = new("Task was cancelled by user", ResultType.Success);

        private readonly ILogger _logger;
        private readonly HospitalDbContext _dbContext;
        private readonly IMapper _mapping;

        public DepartmentService(HospitalDbContext dbContext, IMapper mapping, ILogger<DepartmentService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapping = mapping;
        }

        public async Task<ServiceResult<List<GetDepartmentDTO>>> FilterAsync(FilteringParametersDTO filtering, CancellationToken cancellationToken)
        {
            var departments = _dbContext.Departments
                .AsNoTracking();

            var filter = filtering.Filter?.Trim();

            if (!string.IsNullOrWhiteSpace(filter))
                departments = departments.Where(x => x.LongName.Contains(filter) || x.ShortName.Contains(filter));

            var result = await departments
                .Skip((filtering.Page - 1) * filtering.PageSize)
                .Take(filtering.PageSize)
                .Include(x => x.Patients)
                .ProjectTo<GetDepartmentDTO>(_mapping.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new ServiceResult<List<GetDepartmentDTO>>(result, ResultType.Success);
        }

        public async Task<ServiceResult<GetDepartmentDTO>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var department = await _dbContext.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (department is null)
                return new ServiceResult<GetDepartmentDTO>(data: null, ResultType.NotFound);

            var getDto = _mapping.Map<GetDepartmentDTO>(department);

            return new ServiceResult<GetDepartmentDTO>(getDto, ResultType.Success);
        }

        public async Task<ServiceResult<GetDepartmentDTO>> AddDepartmentAsync(CreateDepartmentDTO dto, CancellationToken cancellationToken)
        {
            var shortName = dto.ShortName.Trim();
            var longName = dto.LongName.Trim();

            var exists = _dbContext.Departments
                .AsNoTracking()
                .Any(x => x.LongName.Equals(longName) || x.ShortName.Equals(shortName));

            if (exists)
            {
                _logger.LogInformation("Department with name '{shortName} ({longName})' already exists", shortName, longName);
                return new ServiceResult<GetDepartmentDTO>(message: $"Department already exists", ResultType.Conflict);
            }

            var newDepartment = new Department();
            try
            {
                newDepartment.Rename(shortName, longName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Department name '{shortName} ({longName})' is not valid", shortName, longName);
                return new ServiceResult<GetDepartmentDTO>(message: "Invalid department names", ResultType.Error);
            }

            try
            {
                await _dbContext.Departments
                    .AddAsync(newDepartment, cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not TaskCanceledException && ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Failed to save department in database");
                return new ServiceResult<GetDepartmentDTO>(message: "Unexpected error during department save", ResultType.Error);
            }

            _logger.LogInformation("Department {shortName} ({longName}) has been saved", shortName, longName);

            var getDto = _mapping.Map<GetDepartmentDTO>(newDepartment);
            return new ServiceResult<GetDepartmentDTO>(getDto, ResultType.Success);
        }

        public async Task<ServiceResult<string>> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var department = await _dbContext.Departments
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (department is null)
                return new ServiceResult<string>(data: null, ResultType.Success);

            var hasAnyPatients = await _dbContext.Patients
                .AnyAsync(patient => patient.DepartmentId == id, cancellationToken);
            if (hasAnyPatients)
                return new ServiceResult<string>("All patients should be re-assigned to other departments in order to delete department", ResultType.Error);

            try
            {
                _dbContext.Departments.Remove(department);

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not TaskCanceledException && ex is not OperationCanceledException)
            {
                _logger.LogError(ex, $"Failed to delete department with unexpected error");
            }

            return new ServiceResult<string>(data: null, ResultType.Success);
        }

        public async Task<ServiceResult<List<GetDepartmentPatientDTO>>> GetOccupationAsync(Guid departmentId, DateOnly? submissionDate, FilteringParametersDTO filtering, CancellationToken cancellationToken)
        {
            try
            {
                var patients = await _dbContext.Patients
                    .AsNoTracking()
                    .Where(x => !x.Discharged && departmentId == x.DepartmentId && (!submissionDate.HasValue || x.AdmissionDate.HasValue && DateOnly.FromDateTime(x.AdmissionDate.Value) == submissionDate))
                    .Skip((filtering.Page - 1) * filtering.PageSize)
                    .Take(filtering.PageSize)
                    .ProjectTo<GetDepartmentPatientDTO>(_mapping.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return new ServiceResult<List<GetDepartmentPatientDTO>>(data: patients, ResultType.Success);
            }
            catch (Exception ex) when (ex is not TaskCanceledException && ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Unexpected exception on patients query");
                return new ServiceResult<List<GetDepartmentPatientDTO>>(message: "Unexpected exception", ResultType.Error);
            }
        }

        public async Task<ServiceResult<AdmitPatientDTO>> AssignAsync(AdmitPatientDTO dto, CancellationToken cancellationToken)
        {
            var admissionNumber = dto.AdmissionNumber?.Trim();

            if (admissionNumber == null)
                return new ServiceResult<AdmitPatientDTO>(message: "Admission number can't be empty", ResultType.Error);

            var patient = await _dbContext.Patients
                .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

            if (patient is null)
                return new ServiceResult<AdmitPatientDTO>(message: "Patient not found", ResultType.NotFound);
            else if (patient.AdmissionNumber == admissionNumber)
                return new ServiceResult<AdmitPatientDTO>(data: _mapping.Map<AdmitPatientDTO>(patient), ResultType.Success, $"Patient is already admissed under same number");
            else if (!patient.Discharged)
                return new ServiceResult<AdmitPatientDTO>(message: $"Another patient is admissed under number {admissionNumber}", ResultType.Conflict);
            else if (patient.DepartmentId == dto.DepartmentId)
                return new ServiceResult<AdmitPatientDTO>("Already in same department", ResultType.Success);

            var department = await _dbContext.Departments
                .FirstOrDefaultAsync(x => x.Id == dto.DepartmentId, cancellationToken);

            if (department is null)
                return new ServiceResult<AdmitPatientDTO>(message: "Department does not exist", ResultType.NotFound);

            patient.Admit(department.Id, admissionNumber, dto.AdmissionDate);

            patient.RowVersion = dto.RowVersion;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ServiceResult<AdmitPatientDTO>(data: _mapping.Map<AdmitPatientDTO>(patient), ResultType.Success);
        }

        public async Task<ServiceResult<GetDepartmentDTO>> UpdateAsync(UpdateDepartmentDTO dto, CancellationToken cancellationToken)
        {
            Department? department;

            department = await _dbContext.Departments
                .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

            if (department is null)
                return new ServiceResult<GetDepartmentDTO>(data: null, ResultType.NotFound);

            var shortName = dto.ShortName.Trim();
            var longName = dto.LongName.Trim();

            if (department.LongName.Equals(longName)
                || department.ShortName.Equals(shortName))
            {
                var departmentExists = await _dbContext.Departments
                    .AnyAsync(x => x.Id != dto.Id && (x.LongName.Equals(longName) || x.ShortName.Equals(shortName)), cancellationToken);

                if (departmentExists)
                    return new ServiceResult<GetDepartmentDTO>(message: "Department with same name already exists", ResultType.Conflict);
            }

            try
            {
                department.Rename(shortName, longName);
                department.RowVersion = dto.RowVersion;
                await _dbContext.SaveChangesAsync(cancellationToken);
                return new ServiceResult<GetDepartmentDTO>(data: _mapping.Map<GetDepartmentDTO>(department), ResultType.Success);
            }
            catch (Exception ex) when (ex is not TaskCanceledException && ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Department '{shortName} ({longName})' update was interrupted before save", shortName, longName);
                return new ServiceResult<GetDepartmentDTO>(message: "Unexpected error occured during saving ", ResultType.Error);
            }
        }
    }
}
