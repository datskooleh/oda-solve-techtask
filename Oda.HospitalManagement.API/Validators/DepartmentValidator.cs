using System.Text.RegularExpressions;

using FluentValidation;

using Oda.HospitalManagement.Application.DTOs.Department;

namespace Oda.HospitalManagement.API.Validators
{
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDTO>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(x => x.ShortName)
                .Length(2, 10)
                .NotEmpty();

            RuleFor(x => x.LongName)
                .Length(5, 50)
                .NotEmpty();
        }
    }

    public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentDTO>
    {
        public UpdateDepartmentValidator()
        {
            RuleFor(x => x.ShortName)
                .Length(2, 10)
                .NotEmpty();

            RuleFor(x => x.LongName)
                .Length(5, 50)
                .NotEmpty();

            RuleFor(x => x.Id)
                .NotEqual(Guid.Empty);
        }
    }

    public sealed class AdmitPatientValidator : AbstractValidator<AdmitPatientDTO>
    {
        private static readonly Regex _admissionNumberFormat = new("^[0-9]{4,6}\\/[0-9]{1,2}");

        public AdmitPatientValidator()
        {
            RuleFor(x => x.AdmissionNumber)
                .MaximumLength(15)
                .Must(x => _admissionNumberFormat.IsMatch(x));

            RuleFor(x => x.DepartmentId)
                .NotEmpty();
        }
    }
}
