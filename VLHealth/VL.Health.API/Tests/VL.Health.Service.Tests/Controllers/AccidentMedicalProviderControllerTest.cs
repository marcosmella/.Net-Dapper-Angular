using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.AccidentMedicalProvider.Response;
using Xunit;


namespace VL.Health.Service.Tests.Controllers
{
    public class AccidentMedicalProviderControllerTest
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly AccidentMedicalProviderController _accidentMedicalProviderController;

        public AccidentMedicalProviderControllerTest()
        {
            _autoMoqer = new AutoMoqer();
            _accidentMedicalProviderController = _autoMoqer.Resolve<AccidentMedicalProviderController>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var accidentMedicalProvider = new List<AccidentMedicalProvider>
            {
                new AccidentMedicalProvider
                {
                    Id = 1,
                    Description = "Broker",
                },
                new AccidentMedicalProvider
                {
                    Id = 2,
                    Description = "ART",
                }
            };

            var accidentMedicalProviderResponse = new List<AccidentMedicalProviderResponse>
            {
                new AccidentMedicalProviderResponse
                {
                    Id = accidentMedicalProvider[0].Id,
                    Description = accidentMedicalProvider[0].Description,
                },
                new AccidentMedicalProviderResponse
                {
                    Id = accidentMedicalProvider[1].Id,
                    Description = accidentMedicalProvider[1].Description,
                }
            };

            var mockAccidentMedicalProviderManager = _autoMoqer.GetMock<IAccidentMedicalProviderManager>();
            mockAccidentMedicalProviderManager.Setup(x => x.Get())
                .Returns(accidentMedicalProvider);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccidentMedicalProviderResponse>>(accidentMedicalProvider))
                .Returns(accidentMedicalProviderResponse);

            //ACT
            var result = _accidentMedicalProviderController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<AccidentMedicalProviderResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < accidentMedicalProvider.Count; i++)
            {
                Assert.Equal(value[i].Id, accidentMedicalProviderResponse[i].Id);
                Assert.Equal(value[i].Description, accidentMedicalProviderResponse[i].Description);
            }
        }
    }
}
