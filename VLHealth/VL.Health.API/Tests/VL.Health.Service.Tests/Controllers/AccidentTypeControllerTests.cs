using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.AccidentType.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class AccidentTypeControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly AccidentTypeController _accidentTypeController;

        public AccidentTypeControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _accidentTypeController = _autoMoqer.Resolve<AccidentTypeController>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var accidentTypes = new List<AccidentType>
            {
                new AccidentType
                {
                    Id = 1,
                    Description = "Accidente In Itinere"
                },
                new AccidentType
                {
                    Id = 2,
                    Description = "Accidente de Trabajo"
                }
            };

            var accidentTypesResponse = new List<AccidentTypeResponse>
            {
                new AccidentTypeResponse
                {
                    Id = accidentTypes[0].Id,
                    Description = accidentTypes[0].Description
                },
                new AccidentTypeResponse
                {
                    Id = accidentTypes[1].Id,
                    Description = accidentTypes[1].Description
                }
            };

            var mockAccidentTypeManager = _autoMoqer.GetMock<IAccidentTypeManager>();
            mockAccidentTypeManager.Setup(x => x.Get())
                .Returns(accidentTypes);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccidentTypeResponse>>(accidentTypes))
                .Returns(accidentTypesResponse);

            //ACT
            var result = _accidentTypeController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<AccidentTypeResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < accidentTypes.Count; i++)
            {
                Assert.Equal(accidentTypesResponse[i].Id, value[i].Id);
                Assert.Equal(accidentTypesResponse[i].Description, value[i].Description);

            }
        }

    }

}
