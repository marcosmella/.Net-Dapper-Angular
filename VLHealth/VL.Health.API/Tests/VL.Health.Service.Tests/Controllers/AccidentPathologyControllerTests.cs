using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.AccidentPathology.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class AccidentPathologyControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly AccidentPathologyController _accidentPathologyController;

        public AccidentPathologyControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _accidentPathologyController = _autoMoqer.Resolve<AccidentPathologyController>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var accidentPathology = new List<AccidentPathology>
            {
                new AccidentPathology
                {
                    Id = 1,
                    Description = "Cervical",
                },
                new AccidentPathology
                {
                    Id = 2,
                    Description = "Craneo",
                }
            };

            var accidentPathologyResponse = new List<AccidentPathologyResponse>
            {
                new AccidentPathologyResponse
                {
                    Id = accidentPathology[0].Id,
                    Description = accidentPathology[0].Description,
                },
                new AccidentPathologyResponse
                {
                    Id = accidentPathology[1].Id,
                    Description = accidentPathology[1].Description,
                }
            };

            var mockAccidentPathologyManager = _autoMoqer.GetMock<IAccidentPathologyManager>();
            mockAccidentPathologyManager.Setup(x => x.Get())
                .Returns(accidentPathology);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccidentPathologyResponse>>(accidentPathology))
                .Returns(accidentPathologyResponse);

            //ACT
            var result = _accidentPathologyController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<AccidentPathologyResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < accidentPathology.Count; i++)
            {
                Assert.Equal(value[i].Id, accidentPathologyResponse[i].Id);
                Assert.Equal(value[i].Description, accidentPathologyResponse[i].Description);
            }
        }
    }
}
