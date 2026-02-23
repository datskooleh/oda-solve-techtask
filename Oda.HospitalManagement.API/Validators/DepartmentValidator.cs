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

            RuleFor(x => x.Id).NotEqual(Guid.Empty);
        }
    }
}
