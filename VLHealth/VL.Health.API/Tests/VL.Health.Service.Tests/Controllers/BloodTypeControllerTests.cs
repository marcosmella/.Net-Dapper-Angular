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
    public class BloodTypeControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly BloodTypeController _bloodTypeController;

        public BloodTypeControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _bloodTypeController = _autoMoqer.Resolve<BloodTypeController>();
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

            var response = new List<BloodTypeResponse>
            {
                new BloodTypeResponse
                {
                    Id = 1,
                    Description = "AB+"
                },
                new BloodTypeResponse
                {
                    Id = 2,
                    Description = "0-"
                }
            };

            var bloodTypeManagerMock = _autoMoqer.GetMock<IBloodTypeManager>();
            bloodTypeManagerMock.Setup(x => x.Get()).Returns(bloodTypes);

            var mapperMock = _autoMoqer.GetMock<IMapper>();
            mapperMock.Setup(x => x.Map<List<BloodTypeResponse>>(bloodTypes)).Returns(response);

            // Act
            var result = _bloodTypeController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<BloodTypeResponse>)model.Value;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);
            for (var i = 0; i < bloodTypes.Count; i++)
            {
                Assert.Equal(response[i].Id, value[i].Id);
                Assert.Equal(response[i].Description, value[i].Description);
            }
        }
    }
}