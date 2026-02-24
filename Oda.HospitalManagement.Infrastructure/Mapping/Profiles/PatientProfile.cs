using AutoMapper;

using Oda.HospitalManagement.Application.DTOs.Department;
using Oda.HospitalManagement.Application.DTOs.Patient;
using Oda.HospitalManagement.Domain;

namespace Oda.HospitalManagement.Infrastructure.Mapping.Profiles
{
    internal class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, GetPatientDTO>();
            CreateMap<Patient, AdmitPatientDTO>();
        }
    }
}
