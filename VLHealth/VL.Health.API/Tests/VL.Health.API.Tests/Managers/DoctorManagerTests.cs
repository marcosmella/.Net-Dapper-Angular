using AutoMoqCore;
using System;
using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.API.Managers;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using Xunit;
using VL.Health.API.Helpers.Interfaces;
using Moq;
using VL.Health.API.Validators.Interfaces;

namespace VL.Health.API.Tests.Managers
{
    public class DoctorManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IDoctorManager _doctorManager;

        public DoctorManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _doctorManager = _autoMoqer.Resolve<DoctorManager>();
        } 

        [Fact]
        public void Get()
        {
            //ARRANGE
            var doctors = new List<Doctor>
            {
                new Doctor
                {
                    Id = 1,
                    FirstName = "Carlos",
                    LastName = "Montero",
                    Enrollment = "1234",
                    DocumentNumber = "223344"
                },
                new Doctor
                {
                    Id = 2,
                    FirstName = "Roberto",
                    LastName = "Perez",
                    Enrollment = "5678",
                    DocumentNumber = "454545"
                }
            };

            var mockDoctorRepositoryValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockDoctorRepositoryValidator.Setup(x => x.ListResult(It.IsAny<Func<List<Doctor>>>())).Returns(doctors);

            //ACT
            var result = _doctorManager.Get();

            //ASSERT
            for (var i = 0; i < doctors.Count; i++)
            {
                Assert.Equal(result[i].Id, doctors[i].Id);
                Assert.Equal(result[i].FirstName, doctors[i].FirstName);
                Assert.Equal(result[i].LastName, doctors[i].LastName);
                Assert.Equal(result[i].Enrollment, doctors[i].Enrollment);
                Assert.Equal(result[i].DocumentNumber, doctors[i].DocumentNumber);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            //ARRANGE
            var mockDoctorRepositoryValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockDoctorRepositoryValidator.Setup(x => x.ListResult(It.IsAny<Func<List<Doctor>>>())).Throws(new FunctionalException(ErrorType.NotFound, "Error"));

            //ACT
            try
            {
                var result = _doctorManager.Get();
            }
            catch (Exception ex)
            {
                //ASSERT
                Assert.Equal(typeof(FunctionalException), ex.GetType());
                Assert.Equal(ErrorType.NotFound, ((FunctionalException)ex).FunctionalError);
            }
        }


        [Fact]
        public void GetById()
        {
            //ARRANGE
            int idDoctor = 5;
            var doctor = new Doctor
            {
                Id = idDoctor,
                FirstName = "Carlos",
                LastName = "Montero",
                Enrollment = "2233",
                EnrollmentExpirationDate = DateTime.Now,
                DocumentNumber = "123456"
            };

            var mockDoctorRepositoryValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockDoctorRepositoryValidator.Setup(x => x.ObjectResult(It.IsAny<Func<int, Doctor>>(), idDoctor)).Returns(doctor);

            //ACT
            var result = _doctorManager.GetDoctor(idDoctor);

            //ASSERT
            Assert.Equal(result.Id, doctor.Id);
            Assert.Equal(result.FirstName, doctor.FirstName);
            Assert.Equal(result.LastName, doctor.LastName);
            Assert.Equal(result.Enrollment, doctor.Enrollment);
            Assert.Equal(result.DocumentNumber, doctor.DocumentNumber);
        }


        [Fact]
        public void CreateDoctor()
        {
            //ARRANGE
            int idDoctor = 2;
            var doctor = new Doctor
            {
                Id = idDoctor,
                FirstName = "Roberto",
                LastName = "Perez",
                Enrollment = "5678",
                DocumentNumber = "454545",
                EnrollmentExpirationDate = DateTime.Now.AddDays(365),
                DocumentExpirationDate = DateTime.Now.AddDays(365)
            };

            bool throwException = true;

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.IntegerResult(It.IsAny<Func<Doctor, int>>(), doctor, throwException)).Returns(idDoctor);

            var mockDoctorValidatory = _autoMoqer.GetMock<ICustomValidator<Doctor>>();
            mockDoctorValidatory.Setup(x => x.IsValid(doctor, ActionType.Create)).Returns(true);

            //ACT
            var result = _doctorManager.Create(doctor);

            //ASSERT
            Assert.Equal(idDoctor, result);
            mockRepositoryResultValidator.Verify(x => x.IntegerResult(It.IsAny<Func<Doctor, int>>(), doctor, throwException), Times.Once());
        }


        [Fact]
        public void UpdateDoctor()
        {
            //ARRANGE
            int idDoctor = 2;
            var doctor = new Doctor
            {
                Id = idDoctor,
                FirstName = "Roberto",
                LastName = "Perez",
                Enrollment = "5678",
                DocumentNumber = "454545",
                EnrollmentExpirationDate = DateTime.Now.AddDays(365),
                DocumentExpirationDate = DateTime.Now.AddDays(365)
            };

            bool throwException = true;

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.IntegerResult(It.IsAny<Func<Doctor, int>>(), doctor, throwException)).Returns(idDoctor);

            var mockDoctorValidator = _autoMoqer.GetMock<ICustomValidator<Doctor>>();
            mockDoctorValidator.Setup(x => x.IsValid(doctor, ActionType.Update)).Returns(true);

            //ACT
            _doctorManager.Update(doctor);

            //ASSERT
            mockRepositoryResultValidator.Verify(x => x.IntegerResult(It.IsAny<Func<Doctor, int>>(), doctor, throwException), Times.Once());
        }

        [Fact]
        public void DeleteDoctor()
        {
            //ARRANGE
            int idDoctor = 2;
            var doctor = new Doctor() { Id = idDoctor };
            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.ObjectResult(It.IsAny<Func<int, Doctor>>(), idDoctor)).Returns(doctor);

            //ACT
            _doctorManager.Delete(idDoctor);

            //ASSERT
            mockRepositoryResultValidator.Verify(x => x.ObjectResult(It.IsAny<Func<int, Doctor>>(), idDoctor), Times.Once());
        }


    }
}
