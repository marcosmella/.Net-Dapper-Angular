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
    public class MedicalControlTrackingTypeManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IMedicalControlTrackingTypeManager _trackingTypeManager;

        public MedicalControlTrackingTypeManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _trackingTypeManager = _autoMoqer.Resolve<MedicalControlTrackingTypeManager>();
        }

        [Fact]
        public void Get()
        {
            // Arrange
            var trackingTypes = new List<MedicalControlTrackingType>
            {
                new MedicalControlTrackingType
                {
                    Id = 1,
                    Description = "Alta de Paciente",
                    CreateAbsence = true
                },
                new MedicalControlTrackingType
                {
                    Id = 2,
                    Description = "Extensión de la Ausencia",
                    CreateAbsence = false
                }
            };

            var trackingTypeRepositoryMock = _autoMoqer.GetMock<IMedicalControlTrackingTypeRepository>();
            
            var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
            helperResultValidatorMock.Setup(x => x.ListResult<MedicalControlTrackingType>(trackingTypeRepositoryMock.Object.Get)).Returns(trackingTypes);

            // Act
            var result = _trackingTypeManager.Get();

            // Assert
            for (var i = 0; i < trackingTypes.Count; i++)
            {
                Assert.Equal(trackingTypes[i].Id, result[i].Id);
                Assert.Equal(trackingTypes[i].Description, result[i].Description);
                Assert.Equal(trackingTypes[i].CreateAbsence, result[i].CreateAbsence);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            // Arranfe
            var trackingTypeRepositoryMock = _autoMoqer.GetMock<IMedicalControlTrackingTypeRepository>();
            trackingTypeRepositoryMock.Setup(x => x.Get()).Returns(new List<MedicalControlTrackingType>());

            // Act
            try
            {
                var result = _trackingTypeManager.Get();
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
