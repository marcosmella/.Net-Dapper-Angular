using AutoMoqCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using VL.Health.API.Exceptions;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.API.Managers;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Managers;
using Xunit;

namespace VL.Health.API.Tests.Managers
{
    public class EmployeeMedicalExamManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IEmployeeMedicalExamManager _employeeMedicalExamManager;

        public EmployeeMedicalExamManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _employeeMedicalExamManager = _autoMoqer.Resolve<EmployeeMedicalExamManager>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var employeeMedicalExams = new List<EmployeeMedicalExam>
            {
                new EmployeeMedicalExam
                {
                    Id = 1,
                    IdEmployee = 16785,
                    IdFileType = 10001,
                    IdFile = 281,
                    ExpirationDate = DateTime.Now
                },
                new EmployeeMedicalExam
                {
                    Id = 2,
                    IdEmployee = 16784,
                    IdFileType = 10001,
                    IdFile = 280,
                    ExpirationDate = null
                }
            };

            var mockEmployeeMedicalExamRepositoryValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockEmployeeMedicalExamRepositoryValidator.Setup(x => x.ListResult(It.IsAny<Func<List<EmployeeMedicalExam>>>())).Returns(employeeMedicalExams);

            //ACT
            var result = _employeeMedicalExamManager.Get();

            //ASSERT
            for (var i = 0; i < employeeMedicalExams.Count; i++)
            {
                Assert.Equal(result[i].Id, employeeMedicalExams[i].Id);
                Assert.Equal(result[i].IdEmployee, employeeMedicalExams[i].IdEmployee);
                Assert.Equal(result[i].IdFileType, employeeMedicalExams[i].IdFileType);
                Assert.Equal(result[i].IdFile, employeeMedicalExams[i].IdFile);
                Assert.Equal(result[i].ExpirationDate, employeeMedicalExams[i].ExpirationDate);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            //ARRANGE
            var mockEmployeeMedicalExamRepositoryValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockEmployeeMedicalExamRepositoryValidator.Setup(x => x.ListResult(It.IsAny<Func<List<EmployeeMedicalExam>>>())).Throws(new FunctionalException(ErrorType.NotFound, "Error"));

            //ACT
            try
            {
                var result = _employeeMedicalExamManager.Get();
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
            int idEmployeeMedicalExam = 3;
            var employeeMedicalExam = new EmployeeMedicalExam
            {
                Id = idEmployeeMedicalExam,
                IdEmployee = 16785,
                IdFileType = 10001,
                IdFile = 281,
                ExpirationDate = DateTime.Now
            };

            var mockEmployeeMedicalExamRepositoryValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockEmployeeMedicalExamRepositoryValidator.Setup(x => x.ObjectResult(It.IsAny<Func<int, EmployeeMedicalExam>>(), idEmployeeMedicalExam)).Returns(employeeMedicalExam);

            //ACT
            var result = _employeeMedicalExamManager.GetEmployeeMedicalExam(idEmployeeMedicalExam);

            //ASSERT
            Assert.Equal(result.Id, employeeMedicalExam.Id);
            Assert.Equal(result.IdEmployee, employeeMedicalExam.IdEmployee);
            Assert.Equal(result.IdFileType, employeeMedicalExam.IdFileType);
            Assert.Equal(result.IdFile, employeeMedicalExam.IdFile);
            Assert.Equal(result.ExpirationDate, employeeMedicalExam.ExpirationDate);
        }


        [Fact]
        public void CreateEmployeeMedicalExam()
        {
            //ARRANGE
            int idEmployeeMedicalExam = 2;
            var employeeMedicalExam = new EmployeeMedicalExam
            {
                Id = idEmployeeMedicalExam,
                IdEmployee = 16785,
                IdFileType = 1001,
                IdFile = 281,
                ExpirationDate = DateTime.Now.AddDays(365)
            };

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.IntegerResult(It.IsAny<Func<EmployeeMedicalExam, int>>(), employeeMedicalExam, true)).Returns(idEmployeeMedicalExam);

            var mockEmployeeMedicalExamValidatory = _autoMoqer.GetMock<ICustomValidator<EmployeeMedicalExam>>();
            mockEmployeeMedicalExamValidatory.Setup(x => x.IsValid(employeeMedicalExam, ActionType.Create)).Returns(true);

            //ACT
            var result = _employeeMedicalExamManager.Create(employeeMedicalExam);

            //ASSERT
            Assert.Equal(idEmployeeMedicalExam, result);
            mockRepositoryResultValidator.Verify(x => x.IntegerResult(It.IsAny<Func<EmployeeMedicalExam, int>>(), employeeMedicalExam, true), Times.Once());
        }


        [Fact]
        public void UpdateEmployeeMedicalExam()
        {
            //ARRANGE
            int idEmployeeMedicalExam = 2;
            var employeeMedicalExam = new EmployeeMedicalExam
            {
                Id = idEmployeeMedicalExam,
                IdEmployee = 16785,
                IdFileType = 10002,
                IdFile = 281,
                ExpirationDate = DateTime.Now.AddDays(365)
            };

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.IntegerResult(It.IsAny<Func<EmployeeMedicalExam, int>>(), employeeMedicalExam, true)).Returns(idEmployeeMedicalExam);

            var mockEmployeeMedicalExamValidator = _autoMoqer.GetMock<ICustomValidator<EmployeeMedicalExam>>();
            mockEmployeeMedicalExamValidator.Setup(x => x.IsValid(employeeMedicalExam, ActionType.Update)).Returns(true);

            //ACT
            _employeeMedicalExamManager.Update(employeeMedicalExam);

            //ASSERT
            mockRepositoryResultValidator.Verify(x => x.IntegerResult(It.IsAny<Func<EmployeeMedicalExam, int>>(), employeeMedicalExam, true), Times.Once());
        }

        [Fact]
        public void DeleteEmployeeMedicalExam()
        {
            //ARRANGE
            int idEmployeeMedicalExam = 2;
            var employeeMedicalExam = new EmployeeMedicalExam() { Id = idEmployeeMedicalExam };
            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.ObjectResult(It.IsAny<Func<int, EmployeeMedicalExam>>(), idEmployeeMedicalExam)).Returns(employeeMedicalExam);

            //ACT
            _employeeMedicalExamManager.Delete(idEmployeeMedicalExam);

            //ASSERT
            mockRepositoryResultValidator.Verify(x => x.ObjectResult(It.IsAny<Func<int, EmployeeMedicalExam>>(), idEmployeeMedicalExam), Times.Once());
        }
    }
}
