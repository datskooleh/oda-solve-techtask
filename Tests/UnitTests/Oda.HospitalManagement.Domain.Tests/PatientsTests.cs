using Oda.HospitalManagement.Domain;
using Oda.HospitalManagement.Domain.Exceptions;

using Shouldly;

namespace Oda.HospitalManagement.UnitTests
{
    public class PatientTests
    {
        private const string Length_51 = "123456789_123456789_123456789_123456789_123456789_1";
        private const string OriginalFirstName = "John";
        private const string OriginalLastName = "Doe";
        private const string AdmissionNumber = "654321/21";

        private static readonly Guid _userId = Guid.Parse("131DF87E-9EDE-4B87-8440-0E46E6217852");
        private static readonly Guid _departmentId = Guid.Parse("339D5C28-8FDE-4ED5-8862-D9C5310E4ED6");
        private static readonly Guid _transferDepartmentId = Guid.Parse("F5C842BD-045A-447D-9726-767B3F7E8A70");

        private static readonly DateTime _admissionDate = DateTime.ParseExact("2026-01-04 13:25:33.035 +01:00", "yyyy-MM-dd HH:mm:ss.fff zzz", null);

        [Fact]
        public void Rename_WhenValidValues_ShouldSucceed()
        {
            var firstName = "firstName";
            var lastName = "lastName";
            var expectedName = $"{lastName}, {firstName}";

            var patient = new Patient(OriginalFirstName, OriginalLastName);
            patient.Rename(firstName, lastName);

            patient.FirstName.ShouldBe(firstName);
            patient.LastName.ShouldBe(lastName);
            patient.Name.ShouldBe(expectedName);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "Doe")]
        [InlineData("John", null)]
        [InlineData("John", Length_51)]
        [InlineData(Length_51, "Doe")]
        public void Rename_WhenParametersInvalid_ShouldThrowException(string? firstName, string? lastName)
        {
            var patient = new Patient(_userId, OriginalFirstName, OriginalLastName);

            Assert.Throws<RenameDomainException>(() => patient.Rename(firstName, lastName));
            patient.FirstName.ShouldBe(OriginalFirstName);
            patient.LastName.ShouldBe(OriginalLastName);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", "1243/1", TestDisplayName = "Department - invalid")]
        [InlineData("449D5C28-8FDE-4ED5-8862-D9C5310E4ED6", "1345", TestDisplayName = "Admission - missing second part")]
        [InlineData("449D5C28-8FDE-4ED5-8862-D9C5310E4ED6", "", TestDisplayName = "Admission - empty")]
        [InlineData("449D5C28-8FDE-4ED5-8862-D9C5310E4ED6", "1345678/13", TestDisplayName = "Admission - invalid first part")]
        [InlineData("449D5C28-8FDE-4ED5-8862-D9C5310E4ED6", "1345TT/13", TestDisplayName = "Admission - invalid alpha first part")]
        [InlineData("449D5C28-8FDE-4ED5-8862-D9C5310E4ED6", "1345/123", TestDisplayName = "Admission - invalid second part")]
        public void Admit_WhenInvalidParameters_ShouldThrowException(Guid departmentId, string? admission)
        {
            var patient = new Patient(_userId, OriginalFirstName, OriginalLastName);

            Assert.Throws<AdmissionDomainException>(() => patient.Admit(departmentId, admission, null));
            patient.AdmissionNumber.ShouldBeNullOrWhiteSpace();
            patient.DepartmentId.HasValue.ShouldBeFalse();
            patient.AdmissionDate.HasValue.ShouldBeFalse();
        }

        [Fact]
        public void Admit_WhenNullAdmissionNumber_ShouldThrowException()
        {
            var patient = new Patient(_userId, OriginalFirstName, OriginalLastName);

            Assert.Throws<NullReferenceException>(() => patient.Admit(_departmentId, null, null));
            patient.AdmissionNumber.ShouldBeNullOrWhiteSpace();
            patient.DepartmentId.HasValue.ShouldBeFalse();
            patient.AdmissionDate.HasValue.ShouldBeFalse();

            patient.FirstName.ShouldBe(OriginalFirstName);
            patient.LastName.ShouldBe(OriginalLastName);
            patient.Id.ShouldBe(_userId);
        }



        [Theory]
        [InlineData("449D5C28-8FDE-4ED5-8862-D9C5310E4ED6", "1234/1")]
        [InlineData("449D5C28-8FDE-4ED5-8862-D9C5310E4ED6", "12345/12")]
        [InlineData("449D5C28-8FDE-4ED5-8862-D9C5310E4ED6", "123456/1")]
        [InlineData("449D5C28-8FDE-4ED5-8862-D9C5310E4ED6", "123456/12")]
        public void Admit_WhenValidParameters_ShouldSucceed(Guid guid, string admissionNumber)
        {
            var expectedDateTime = DateTime.UtcNow;

            var patient = new Patient(_userId, OriginalFirstName, OriginalLastName);

            patient.Admit(guid, admissionNumber, expectedDateTime);
            patient.AdmissionNumber.ShouldBe(admissionNumber);
            patient.DepartmentId.ShouldBe(guid);
            patient.AdmissionDate.ShouldBe(expectedDateTime);

            patient.FirstName.ShouldBe(OriginalFirstName);
            patient.LastName.ShouldBe(OriginalLastName);
            patient.Id.ShouldBe(_userId);
        }

        [Fact]
        public void Transfer_WhenInDepartment_ShouldSucceed()
        {
            var patient = new Patient(_userId, OriginalFirstName, OriginalLastName);
            patient.Admit(_departmentId, AdmissionNumber, _admissionDate);

            patient.Transfer(_transferDepartmentId);
        }

        [Fact]
        public void Transfer_WhenNotAdmitted_ShouldThrowException()
        {
            var patient = new Patient(_userId, OriginalFirstName, OriginalLastName);
            Assert.Throws<AdmissionDomainException>(() => patient.Transfer(_transferDepartmentId));
        }

        [Fact]
        public void Transfer_WhenInvalidDepartment_ShouldThrowException()
        {
            var patient = new Patient(_userId, OriginalFirstName, OriginalLastName);
            patient.Admit(_departmentId, AdmissionNumber, _admissionDate);
            Assert.Throws<AdmissionDomainException>(() => patient.Transfer(Guid.Empty));
        }

        [Fact]
        public void Discharge_WhenAdmitted_ShouldSucceed()
        {
            var expectedDischargeDate = DateTime.UtcNow;

            var patient = new Patient(_userId, OriginalFirstName, OriginalLastName);
            patient.Admit(_departmentId, AdmissionNumber, _admissionDate);

            patient.Discharge(expectedDischargeDate);

            patient.DischargeDate.ShouldBe(expectedDischargeDate);
        }

        [Fact]
        public void Discharge_WhenDischarged_ShouldThrowException()
        {
            var expectedDischargeDate = DateTime.UtcNow;

            var patient = new Patient(_userId, OriginalFirstName, OriginalLastName);
            patient.Admit(_departmentId, AdmissionNumber, _admissionDate);

            patient.Discharge(expectedDischargeDate);
            Assert.Throws<DischargeDomainException>(() => patient.Discharge(expectedDischargeDate));

            patient.Discharged.ShouldBe(true);
        }

        [Fact]
        public void Discharge_WhenNotAdmitted_ShouldThrowException()
        {
            var patient = new Patient(_userId, OriginalFirstName, OriginalLastName);
            Assert.Throws<DischargeDomainException>(() => patient.Discharge(DateTime.UtcNow));

            patient.FirstName.ShouldBe(OriginalFirstName);
            patient.LastName.ShouldBe(OriginalLastName);
            patient.Id.ShouldBe(_userId);
        }

        [Fact]
        public void Discharge_WhenBeforeAdmission_ShouldThrowException()
        {
            var dischargeDateDayBeforeAdmission = _admissionDate.AddDays(-1);

            var patient = new Patient(_userId, OriginalFirstName, OriginalLastName);
            patient.Admit(_departmentId, AdmissionNumber, _admissionDate);
            Assert.Throws<DischargeDomainException>(() => patient.Discharge(dischargeDateDayBeforeAdmission));

            patient.FirstName.ShouldBe(OriginalFirstName);
            patient.LastName.ShouldBe(OriginalLastName);
            patient.Id.ShouldBe(_userId);
        }

        [Fact]
        public void Discharge_WhenInFuture_ShouldThrowException()
        {
            var dischargeDateDayInFuture = DateTime.UtcNow.AddMinutes(5);

            var patient = new Patient(_userId, OriginalFirstName, OriginalLastName);
            patient.Admit(_departmentId, AdmissionNumber, _admissionDate);
            Assert.Throws<DischargeDomainException>(() => patient.Discharge(dischargeDateDayInFuture));

            patient.FirstName.ShouldBe(OriginalFirstName);
            patient.LastName.ShouldBe(OriginalLastName);
            patient.Id.ShouldBe(_userId);
        }
    }
}
