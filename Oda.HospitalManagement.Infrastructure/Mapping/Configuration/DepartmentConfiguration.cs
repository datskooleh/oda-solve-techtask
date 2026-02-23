using AutoMapper;

using Oda.HospitalManagement.Infrastructure.Mapping.Profiles;

namespace Oda.HospitalManagement.Infrastructure.Mapping.Configuration
{
    internal static class DepartmentConfiguration
    {
        public static MapperConfiguration GetConfiguration()
            => new MapperConfiguration(config =>
            {
                config.AddProfile<DepartmentProfile>();
            }, null);
    }
}
