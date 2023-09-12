using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.BloodPressure.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class BloodPressureControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly BloodPressureController _bloodPressureController;

        public BloodPressureControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _bloodPressureController = _autoMoqer.Resolve<BloodPressureController>();
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

            var response = new List<BloodPressureResponse>
            {
                new BloodPressureResponse
                {
                    Id = 1,
                    Description = "Normal"
                },
                new BloodPressureResponse
                {
                    Id = 2,
                    Description = "Baja"
                },
                new BloodPressureResponse
                {
                    Id = 3,
                    Description = "Alta"
                }
            };

            var bloodPressureManagerMock = _autoMoqer.GetMock<IBloodPressureManager>();
            bloodPressureManagerMock.Setup(x => x.Get()).Returns(bloodPressures);

            var mapperMock = _autoMoqer.GetMock<IMapper>();
            mapperMock.Setup(x => x.Map<List<BloodPressureResponse>>(bloodPressures)).Returns(response);

            // Act
            var result = _bloodPressureController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<BloodPressureResponse>)model.Value;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);
            for (var i = 0; i < bloodPressures.Count; i++)
            {
                Assert.Equal(response[i].Id, value[i].Id);
                Assert.Equal(response[i].Description, value[i].Description);
            }
        }
    }
}