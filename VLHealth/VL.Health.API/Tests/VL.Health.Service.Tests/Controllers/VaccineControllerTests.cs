using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.Vaccine.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class VaccineControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly VaccineController _vaccineController;

        public VaccineControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _vaccineController = _autoMoqer.Resolve<VaccineController>();
        }

        [Fact]
        public void Get()
        {
            // Arrange
            var vaccines = new List<Vaccine>
            {
                new Vaccine
                {
                    Id = 1,
                    Description = "COVID-19 - AstraZeneca"
                },
                new Vaccine
                {
                    Id = 2,
                    Description = "Hepatitis B"
                },
                new Vaccine
                {
                    Id = 3,
                    Description = "Triple Viral"
                }
            };

            var response = new List<VaccineResponse>
            {
                 new VaccineResponse
                {
                    Id = 1,
                    Description = "COVID-19 - AstraZeneca"
                },
                new VaccineResponse
                {
                    Id = 2,
                    Description = "Hepatitis B"
                },
                new VaccineResponse
                {
                    Id = 3,
                    Description = "Triple Viral"
                }
            };

            var vaccineManagerMock = _autoMoqer.GetMock<IVaccineManager>();
            vaccineManagerMock.Setup(x => x.Get()).Returns(vaccines);

            var mapperMock = _autoMoqer.GetMock<IMapper>();
            mapperMock.Setup(x => x.Map<List<VaccineResponse>>(vaccines)).Returns(response);

            // Act
            var result = _vaccineController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<VaccineResponse>)model.Value;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);
            for (var i = 0; i < vaccines.Count; i++)
            {
                Assert.Equal(response[i].Id, value[i].Id);
                Assert.Equal(response[i].Description, value[i].Description);
            }
        }
    }
}
