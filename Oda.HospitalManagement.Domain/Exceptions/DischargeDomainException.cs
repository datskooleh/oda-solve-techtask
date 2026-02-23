namespace Oda.HospitalManagement.Domain.Exceptions
{
    public sealed class DischargeDomainException : Exception
    {
        public DischargeDomainException(string message)
            : base(message)
        { }
    }
}
