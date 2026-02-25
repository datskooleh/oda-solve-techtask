using Oda.HospitalManagement.API.Validators;
using Oda.HospitalManagement.Application.DTOs.Department;

using Shouldly;

namespace Oda.HospitalManagement.APITests
{
    public class DepartmentValidatorTests
    {
        private const string Length11 = "123456789_1";
        private const string Length_51 = "123456789_123456789_123456789_123456789_123456789_1";

        private const string ShortName = "GASTRO";
        private const string LongName = "Gastroenterologie";

        [Fact]
        public void CreateValidation_ValidValues_ShouldSucceed()
        {
            var validator = new CreateDepartmentValidator();

            var dto = new CreateDepartmentDTO(ShortName, LongName);
            var result = validator.Validate(dto);
            result.IsValid.ShouldBeTrue();
        }

        [Theory]
        [InlineData(Length11, LongName)]
        [InlineData(null, LongName)]
        [InlineData("", LongName)]
        [InlineData(ShortName, Length_51)]
        [InlineData(ShortName, null)]
        [InlineData(ShortName, "")]
        public void CreateValidation_InvalidValues_ShouldFail(string? shortName, string? longName)
        {
            var validator = new CreateDepartmentValidator();

            var dto = new CreateDepartmentDTO(shortName, longName);
            var result = validator.Validate(dto);
            result.IsValid.ShouldBeFalse();
        }
    }
}
