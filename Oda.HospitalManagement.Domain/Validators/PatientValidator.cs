using System.Text.RegularExpressions;

using Oda.HospitalManagement.Domain.Exceptions;

namespace Oda.HospitalManagement.Domain.Validators
{
    public static class PatientValidator
    {
        public static int FirstNameMaxLength => 50;

        public static int LastNameMaxLength => 50;

        public static int AdmissionNumberMaxLength => 15;

        public static Regex AdmissionNumberFormat => new("^[0-9]{4,6}\\/[0-9]{1,2}");

        internal static void EnsureAdmissionNumberIsValid(this string admissionNumber)
        {
            if (string.IsNullOrWhiteSpace(admissionNumber) || admissionNumber.Length > AdmissionNumberMaxLength)
                throw new AdmissionDomainException($"Admission number exceeds allowed amount of {AdmissionNumberMaxLength}");

            if (!AdmissionNumberFormat.IsMatch(admissionNumber))
                throw new AdmissionDomainException($"Invalid format of admission number. Admission number should be in format nnNNNN/Nn");
        }

        internal static void EnsureDischargeIsValid(bool discharged, DateTime dischargeDate, DateTime? admissionDate)
        {
            if (!admissionDate.HasValue)
                throw new DischargeDomainException("Patient was never admissed");

            if (discharged)
                throw new DischargeDomainException("Patient is already discharged");

            if (dischargeDate < admissionDate)
                throw new DischargeDomainException("Discharge date should not be latern than admission date");
        }
    }
}
