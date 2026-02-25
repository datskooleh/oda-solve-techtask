using Oda.HospitalManagement.Domain;
using Oda.HospitalManagement.Domain.Exceptions;

using Shouldly;

namespace Oda.HospitalManagement.UnitTests
{
    public class DepartmentTests
    {
        private const string Length11 = "123456789_1";
        private const string Length_51 = "123456789_123456789_123456789_123456789_123456789_1";
        private const string DepartmentShortName = "AWR";
        private const string DepartmentLongName = "Aufwachraum";

        [Fact]
        public void Rename_WhenValid_ShouldSucceed()
        {
            var expectedName = $"{DepartmentShortName} ({DepartmentLongName})";

            var department = new Department();
            department.Rename(DepartmentShortName, DepartmentLongName);
            
            department.ShortName.ShouldBe(DepartmentShortName);
            department.LongName.ShouldBe(DepartmentLongName);
            department.Name.ShouldBe(expectedName);
        }

        [Theory]
        [InlineData(Length11, DepartmentLongName, TestDisplayName = "ShortName - exceeds length")]
        [InlineData("", DepartmentLongName, TestDisplayName = "ShortName - empty")]
        [InlineData(DepartmentShortName, Length_51, TestDisplayName = "LongName - exceeds length")]
        [InlineData(DepartmentShortName, "", TestDisplayName = "LongName - empty")]
        public void Rename_WhenInvalid_ShouldThrowException(string? shortName, string? longName)
        {
            var department = new Department();

            Assert.Throws<RenameDomainException>(() => department.Rename(shortName, longName));

            department.ShortName.ShouldBeNullOrWhiteSpace();
            department.LongName.ShouldBeNullOrWhiteSpace();
            department.Name.ShouldBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null, DepartmentLongName, TestDisplayName = "ShortName - null")]
        [InlineData(DepartmentShortName, null, TestDisplayName = "LongName - null")]
        public void Rename_WhenNull_ShouldThrowException(string? shortName, string? longName)
        {
            var department = new Department();

            Assert.Throws<NullReferenceException>(() => department.Rename(shortName, longName));

            department.ShortName.ShouldBeNullOrWhiteSpace();
            department.LongName.ShouldBeNullOrWhiteSpace();
            department.Name.ShouldBeNullOrWhiteSpace();
        }
    }
}
