namespace Oda.HospitalManagement.Domain.Exceptions
{
    public sealed class AdmissionDomainException : Exception
    {
        public AdmissionDomainException(string message)
            : base(message)
        { }
    }
}
