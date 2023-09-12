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
using VL.Health.Interfaces.Repositories;
using Xunit;

namespace VL.Health.API.Tests.Managers
{
    public class EmployeeMedicalHistoryManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly EmployeeMedicalHistoryManager _medicalHistoryManager;

        public EmployeeMedicalHistoryManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _medicalHistoryManager = _autoMoqer.Resolve<EmployeeMedicalHistoryManager>();
        }

        [Fact]
        public void Get()
        {
            // Arrange
            int idPerson = 647;
            int idBloodPressure = 2;
            int idBloodType = 3;
            bool isRiskGroup = false;

            var medicalHistory = new EmployeeMedicalHistory()
            {
                IdPerson = idPerson,
                IdBloodPressure = idBloodPressure,
                IdBloodType = idBloodType,
                IsRiskGroup = isRiskGroup
            };

            var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
            helperResultValidatorMock.Setup(x => x.ObjectResult<EmployeeMedicalHistory>(It.IsAny<Func<int, EmployeeMedicalHistory>>(), idPerson)).Returns(medicalHistory);

            // Act
            var value = _medicalHistoryManager.Get(idPerson);

            // Assert
            Assert.Equal(medicalHistory, value);
        }

        [Fact]
        public void Create()
        {
            // Arrange
            int id = 1;
            int idPerson = 647;
            int idBloodPressure = 2;
            int idBloodType = 3;
            bool isRiskGroup = false;

            var medicalHistory = new EmployeeMedicalHistory()
            {
                IdPerson = idPerson,
                IdBloodPressure = idBloodPressure,
                IdBloodType = idBloodType,
                IsRiskGroup = isRiskGroup
            };

            bool throwException = true;

            var medicalHistoryValidatoryMock = _autoMoqer.GetMock<ICustomValidator<EmployeeMedicalHistory>>();
            medicalHistoryValidatoryMock.Setup(x => x.IsValid(medicalHistory, ActionType.Create)).Returns(true);

            var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
            helperResultValidatorMock.Setup(x => x.IntegerResult<EmployeeMedicalHistory>(It.IsAny<Func<EmployeeMedicalHistory, int>>(), medicalHistory, throwException)).Returns(id);

            // Act
            int id_inserted = _medicalHistoryManager.Create(medicalHistory);

            // Assert
            Assert.Equal(id, id_inserted);
            medicalHistoryValidatoryMock.Verify(x => x.IsValid(medicalHistory, ActionType.Create), Times.Once);
            helperResultValidatorMock.Verify(x => x.IntegerResult<EmployeeMedicalHistory>(It.IsAny<Func<EmployeeMedicalHistory, int>>(), medicalHistory, throwException), Times.Once);
        }

        [Fact]
        public void Create_When_Validation_Fails_Should_Throw_Exception()
        {
            // Arrange
            int id = 1;
            int idPerson = 647;
            int idBloodPressure = 2;
            int idBloodType = 3;
            bool isRiskGroup = false;

            var medicalHistory = new EmployeeMedicalHistory()
            {
                IdPerson = idPerson,
                IdBloodPressure = idBloodPressure,
                IdBloodType = idBloodType,
                IsRiskGroup = isRiskGroup
            };

            var medicalHistoryRepositoryMock = _autoMoqer.GetMock<IEmployeeMedicalHistoryRepository>();

            var medicalHistoryValidatoryMock = _autoMoqer.GetMock<ICustomValidator<EmployeeMedicalHistory>>();
            medicalHistoryValidatoryMock.Setup(x => x.IsValid(medicalHistory, ActionType.Create)).Returns(false);

            bool throwException = true;

            var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
            helperResultValidatorMock.Setup(x => x.IntegerResult<EmployeeMedicalHistory>(It.IsAny<Func<EmployeeMedicalHistory, int>>(), medicalHistory, throwException)).Returns(id);

			// Act
			try
			{
                int id_inserted = _medicalHistoryManager.Create(medicalHistory);
            }
            catch (Exception e)
			{
                // Assert
                Assert.IsType<FunctionalException>(e);
                medicalHistoryValidatoryMock.Verify(x => x.IsValid(medicalHistory, ActionType.Create), Times.Once);
                helperResultValidatorMock.Verify(x => x.IntegerResult<EmployeeMedicalHistory>(It.IsAny<Func<EmployeeMedicalHistory, int>>(), medicalHistory, throwException), Times.Never);
            }
        }

        [Fact]
        public void Update()
        {
            // Arrange
            int idPerson = 647;
            int idBloodPressure = 2;
            int idBloodType = 3;
            bool isRiskGroup = false;

            var medicalHistory = new EmployeeMedicalHistory()
            {
                IdPerson = idPerson,
                IdBloodPressure = idBloodPressure,
                IdBloodType = idBloodType,
                IsRiskGroup = isRiskGroup
            };

            bool throwException = true;

            var medicalHistoryValidatoryMock = _autoMoqer.GetMock<ICustomValidator<EmployeeMedicalHistory>>();
            medicalHistoryValidatoryMock.Setup(x => x.IsValid(medicalHistory, ActionType.Update)).Returns(true);

            var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
            helperResultValidatorMock.Setup(x => x.IntegerResult<EmployeeMedicalHistory>(It.IsAny<Func<EmployeeMedicalHistory, int>>(), medicalHistory, throwException));

            // Act
            _medicalHistoryManager.Update(medicalHistory);

            // Assert
            medicalHistoryValidatoryMock.Verify(x => x.IsValid(medicalHistory, ActionType.Update), Times.Once);
            helperResultValidatorMock.Verify(x => x.IntegerResult<EmployeeMedicalHistory>(It.IsAny<Func<EmployeeMedicalHistory, int>>(), medicalHistory, throwException), Times.Once);
        }

        [Fact]
        public void Update_When_Validation_Fails_Should_Throw_Exception()
        {
            // Arrange
            int idPerson = 647;
            int idBloodPressure = 2;
            int idBloodType = 3;
            bool isRiskGroup = false;

            var medicalHistory = new EmployeeMedicalHistory()
            {
                IdPerson = idPerson,
                IdBloodPressure = idBloodPressure,
                IdBloodType = idBloodType,
                IsRiskGroup = isRiskGroup
            };

            bool throwException = true;

            var medicalHistoryValidatoryMock = _autoMoqer.GetMock<ICustomValidator<EmployeeMedicalHistory>>();
            medicalHistoryValidatoryMock.Setup(x => x.IsValid(medicalHistory, ActionType.Update)).Returns(false);

            var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
            helperResultValidatorMock.Setup(x => x.IntegerResult<EmployeeMedicalHistory>(It.IsAny<Func<EmployeeMedicalHistory, int>>(), medicalHistory, throwException));

            // Act
            try
            {
                _medicalHistoryManager.Update(medicalHistory);
            }
            catch (Exception e)
            {
                // Assert
                Assert.IsType<FunctionalException>(e);
                medicalHistoryValidatoryMock.Verify(x => x.IsValid(medicalHistory, ActionType.Update), Times.Once);
                helperResultValidatorMock.Verify(x => x.IntegerResult<EmployeeMedicalHistory>(It.IsAny<Func<EmployeeMedicalHistory, int>>(), medicalHistory, throwException), Times.Never);
            }
        }
    }
}
