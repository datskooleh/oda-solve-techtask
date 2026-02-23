namespace Oda.HospitalManagement.Domain.Exceptions
{
    public sealed class RenameDomainException : Exception
    {
        public RenameDomainException(string message)
            : base(message)
        { }
    }
}
