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
    public class BloodTypeManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IBloodTypeManager _bloodTypeManager;

        public BloodTypeManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _bloodTypeManager = _autoMoqer.Resolve<BloodTypeManager>();
        }

        [Fact]
        public void Get()
        {
            // Arrange
            var bloodTypes = new List<BloodType>
            {
                new BloodType
                {
                    Id = 1,
                    Description = "AB+"
                },
                new BloodType
                {
                    Id = 2,
                    Description = "0-"
                }
            };

            var bloodTypeRepositoryMock = _autoMoqer.GetMock<IBloodTypeRepository>();
            
            var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
            helperResultValidatorMock.Setup(x => x.ListResult<BloodType>(bloodTypeRepositoryMock.Object.Get)).Returns(bloodTypes);

            // Act
            var result = _bloodTypeManager.Get();

            // Assert
            for (var i = 0; i < bloodTypes.Count; i++)
            {
                Assert.Equal(bloodTypes[i].Id, result[i].Id);
                Assert.Equal(bloodTypes[i].Description, result[i].Description);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            // Arranfe
            var bloodTypeRepositoryMock = _autoMoqer.GetMock<IBloodTypeRepository>();
            bloodTypeRepositoryMock.Setup(x => x.Get()).Returns(new List<BloodType>());

            // Act
            try
            {
                var result = _bloodTypeManager.Get();
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
