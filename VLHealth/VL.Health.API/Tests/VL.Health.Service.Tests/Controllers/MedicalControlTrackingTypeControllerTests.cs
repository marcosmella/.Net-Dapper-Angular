using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.BloodType.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class MedicalControlTrackingTypeControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly MedicalControlTrackingTypeController _trackingTypeController;

        public MedicalControlTrackingTypeControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _trackingTypeController = _autoMoqer.Resolve<MedicalControlTrackingTypeController>();
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

            var response = new List<MedicalControlTrackingTypeResponse>
            {
                new MedicalControlTrackingTypeResponse
                {
                    Id = 1,
                    Description = "Alta de Paciente",
                    CreateAbsence = true
                },
                new MedicalControlTrackingTypeResponse
                {
                    Id = 2,
                    Description = "Reapertura",
                    CreateAbsence = true,
                }
            };

            var trackingTypeManagerMock = _autoMoqer.GetMock<IMedicalControlTrackingTypeManager>();
            trackingTypeManagerMock.Setup(x => x.Get()).Returns(trackingTypes);

            var mapperMock = _autoMoqer.GetMock<IMapper>();
            mapperMock.Setup(x => x.Map<List<MedicalControlTrackingTypeResponse>>(trackingTypes)).Returns(response);

            // Act
            var result = _trackingTypeController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<MedicalControlTrackingTypeResponse>)model.Value;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);
            for (var i = 0; i < trackingTypes.Count; i++)
            {
                Assert.Equal(response[i].Id, value[i].Id);
                Assert.Equal(response[i].Description, value[i].Description);
                Assert.Equal(response[i].CreateAbsence, value[i].CreateAbsence);
            }
        }
    }
}