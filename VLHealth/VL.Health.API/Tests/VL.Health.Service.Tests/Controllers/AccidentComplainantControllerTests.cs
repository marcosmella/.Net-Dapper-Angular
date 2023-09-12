using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.AccidentComplainant.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class AccidentComplainantControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly AccidentComplainantController _accidentComplainantController;

        public AccidentComplainantControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _accidentComplainantController = _autoMoqer.Resolve<AccidentComplainantController>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var accidentComplainants = new List<AccidentComplainant>
            {
                new AccidentComplainant
                {
                    Id = 1,
                    Description = "Analista",
                },
                new AccidentComplainant
                {
                    Id = 2,
                    Description = "Cuenta",
                }
            };

            var accidentComplainantResponse = new List<AccidentComplainantResponse>
            {
                new AccidentComplainantResponse
                {
                    Id = accidentComplainants[0].Id,
                    Description = accidentComplainants[0].Description,
                },
                new AccidentComplainantResponse
                {
                    Id = accidentComplainants[1].Id,
                    Description = accidentComplainants[1].Description,
                }
            };

            var mockAccidentComplainantManager = _autoMoqer.GetMock<IAccidentComplainantManager>();
            mockAccidentComplainantManager.Setup(x => x.Get())
                .Returns(accidentComplainants);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccidentComplainantResponse>>(accidentComplainants))
                .Returns(accidentComplainantResponse);

            //ACT
            var result = _accidentComplainantController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<AccidentComplainantResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < accidentComplainants.Count; i++)
            {
                Assert.Equal(value[i].Id, accidentComplainantResponse[i].Id);
                Assert.Equal(value[i].Description, accidentComplainantResponse[i].Description);
            }
        }
    }
}
