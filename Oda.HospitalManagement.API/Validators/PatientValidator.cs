using FluentValidation;

using Oda.HospitalManagement.Application.DTOs.Patient;

namespace Oda.HospitalManagement.API.Validators
{
    internal static class PatientValidatorLimits
    {
        internal static int MaxFirstNameLength = 50;
        internal static int MaxLastNameLength = 50;
    }

    public sealed class CreatePatientValidator : AbstractValidator<CreatePatientDTO>
    {
        public CreatePatientValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(PatientValidatorLimits.MaxFirstNameLength);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(PatientValidatorLimits.MaxLastNameLength);

            RuleFor(x => x.AdmissionNumber)
                .MaximumLength(15);
        }
    }

    public sealed class UpdatePatientValidator : AbstractValidator<UpdatePatientDTO>
    {
        public UpdatePatientValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(PatientValidatorLimits.MaxFirstNameLength);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(PatientValidatorLimits.MaxLastNameLength);
        }
    }

    public sealed class AdmitPatientValidator : AbstractValidator<AdmitPatientToDepartmentDTO>
    {
        public AdmitPatientValidator()
        {
            RuleFor(x => x.AdmissionNumber)
                .MaximumLength(15);
        }
    }
}
