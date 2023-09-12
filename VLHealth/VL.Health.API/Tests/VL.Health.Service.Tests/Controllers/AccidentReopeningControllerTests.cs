using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.AccidentReopening.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class AccidentReopeningControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly AccidentReopeningController _accidentReopeningController;

        public AccidentReopeningControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _accidentReopeningController = _autoMoqer.Resolve<AccidentReopeningController>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var accidentReopenings = new List<AccidentReopening>
            {
                new AccidentReopening
                {
                    Id = 1,
                    Description = "Espontanea"

                },
                new AccidentReopening
                {
                    Id = 2,
                    Description = "Servicio Medico"
                }
            };

            var accidentReopeningsResponse = new List<AccidentReopeningResponse>
            {
                new AccidentReopeningResponse
                {
                    Id = accidentReopenings[0].Id,
                    Description = accidentReopenings[0].Description
                },
                new AccidentReopeningResponse
                {
                    Id = accidentReopenings[1].Id,
                    Description = accidentReopenings[1].Description
                }
            };

            var mockAccidentReopeningManager = _autoMoqer.GetMock<IAccidentReopeningManager>();
            mockAccidentReopeningManager.Setup(x => x.Get())
                .Returns(accidentReopenings);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccidentReopeningResponse>>(accidentReopenings))
                .Returns(accidentReopeningsResponse);

            //ACT
            var result = _accidentReopeningController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<AccidentReopeningResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < accidentReopenings.Count; i++)
            {
                Assert.Equal(value[i].Id, accidentReopeningsResponse[i].Id);
                Assert.Equal(value[i].Description, accidentReopeningsResponse[i].Description);
            }
        }
    }
}
