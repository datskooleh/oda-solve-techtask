using Oda.HospitalManagement.Domain.Exceptions;

namespace Oda.HospitalManagement.Domain.Validators
{
    public static class DepartmentValidator
    {
        public static int ShortNameMaxLength => 10;

        public static int LongNameMaxLength => 50;
    }
}
