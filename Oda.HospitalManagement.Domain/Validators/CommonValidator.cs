using Oda.HospitalManagement.Domain.Exceptions;

namespace Oda.HospitalManagement.Domain.Validators
{
    internal static class CommonValidator
    {
        internal static void EnsureValidName(this string name, string nameText, int length)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new RenameDomainException($"{nameText} should not be empty");
            else if (name.Length > length)
                throw new RenameDomainException($"{nameText} should not be bigger than {length} characters");
        }
    }
}
