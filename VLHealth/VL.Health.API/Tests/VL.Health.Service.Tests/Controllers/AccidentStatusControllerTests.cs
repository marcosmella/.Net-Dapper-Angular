using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.AccidentStatus.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class AccidentStatusControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly AccidentStatusController _accidentStatusController;

        public AccidentStatusControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _accidentStatusController = _autoMoqer.Resolve<AccidentStatusController>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var accidentStatuses = new List<AccidentStatus>
            {
                new AccidentStatus
                {
                    Id = 1,
                    Description = "AA",
                },
                new AccidentStatus
                {
                    Id = 2,
                    Description = "BB",
                }
            };

            var accidentStatusResponse = new List<AccidentStatusResponse>
            {
                new AccidentStatusResponse
                {
                    Id = accidentStatuses[0].Id,
                    Description = accidentStatuses[0].Description,
                },
                new AccidentStatusResponse
                {
                    Id = accidentStatuses[1].Id,
                    Description = accidentStatuses[1].Description,
                }
            };

            var mockAccidentStatusManager = _autoMoqer.GetMock<IAccidentStatusManager>();
            mockAccidentStatusManager.Setup(x => x.Get())
                .Returns(accidentStatuses);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccidentStatusResponse>>(accidentStatuses))
                .Returns(accidentStatusResponse);

            //ACT
            var result = _accidentStatusController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<AccidentStatusResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < accidentStatuses.Count; i++)
            {
                Assert.Equal(value[i].Id, accidentStatusResponse[i].Id);
                Assert.Equal(value[i].Description, accidentStatusResponse[i].Description);
            }
        }
    }
}
