using FluentValidation;

using Oda.HospitalManagement.Application.DTOs;

namespace Oda.HospitalManagement.API.Validators
{
    internal sealed class FilteringValidator : AbstractValidator<FilteringParametersDTO>
    {
        internal static int MinimumPage => 0;
        internal static int MinimumPageSize => 10;
        internal static int MaximumFilterLength => 30;
        internal static string MaximumFilterLengthMessage => $"Filter should not exceed {MaximumFilterLength}";

        public FilteringValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(MinimumPage);

            RuleFor(x => x.PageSize)
                .GreaterThan(MinimumPageSize);

            RuleFor(x => x.Filter)
                .MaximumLength(MaximumFilterLength)
                .WithMessage(MaximumFilterLengthMessage);
        }
    }
}
