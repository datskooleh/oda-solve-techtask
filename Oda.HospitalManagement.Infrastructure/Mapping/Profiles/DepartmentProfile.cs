using AutoMapper;

using Oda.HospitalManagement.Application.DTOs.Department;
using Oda.HospitalManagement.Domain;

namespace Oda.HospitalManagement.Infrastructure.Mapping.Profiles
{
    internal sealed class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, GetDepartmentDTO>();
            CreateMap<Patient, GetDepartmentPatientDTO>();
        }
    }
}
