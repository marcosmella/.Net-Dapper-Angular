using AutoMoqCore;
using System;
using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.API.Managers;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;
using Xunit;
using VL.Health.API.Helpers.Interfaces;

namespace VL.Health.API.Tests.Managers
{
    public class BloodPressureManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IBloodPressureManager _bloodPressureManager;

        public BloodPressureManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _bloodPressureManager = _autoMoqer.Resolve<BloodPressureManager>();
        }

        [Fact]
        public void Get()
        {
            // Arrange
            var bloodPressures = new List<BloodPressure>
            {
                new BloodPressure
                {
                    Id = 1,
                    Description = "Normal"
                },
                new BloodPressure
                {
                    Id = 2,
                    Description = "Baja"
                },
                new BloodPressure
                {
                    Id = 3,
                    Description = "Alta"
                }
            };

            var bloodPressureRepositoryMock = _autoMoqer.GetMock<IBloodPressureRepository>();

            var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
            helperResultValidatorMock.Setup(x => x.ListResult<BloodPressure>(bloodPressureRepositoryMock.Object.Get)).Returns(bloodPressures);

            // Act
            var result = _bloodPressureManager.Get();

            // Assert
            for (var i = 0; i < bloodPressures.Count; i++)
            {
                Assert.Equal(bloodPressures[i].Id, result[i].Id);
                Assert.Equal(bloodPressures[i].Description, result[i].Description);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            // Arranfe
            var bloodPressureRepositoryMock = _autoMoqer.GetMock<IBloodPressureRepository>();
            bloodPressureRepositoryMock.Setup(x => x.Get()).Returns(new List<BloodPressure>());

            // Act
            try
            {
                var result = _bloodPressureManager.Get();
            }
            catch (Exception ex)
            {
                // Assert
                Assert.Equal(typeof(FunctionalException), ex.GetType());
                Assert.Equal(ErrorType.NotFound, ((FunctionalException)ex).FunctionalError);
            }
        }
    }
}
