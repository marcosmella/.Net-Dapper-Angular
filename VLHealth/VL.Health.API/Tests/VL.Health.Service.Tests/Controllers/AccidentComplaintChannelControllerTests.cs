using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.AccidentComplaintChannel.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class AccidentComplaintChannelControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly AccidentComplaintChannelController _accidentComplaintChannelController;

        public AccidentComplaintChannelControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _accidentComplaintChannelController = _autoMoqer.Resolve<AccidentComplaintChannelController>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var accidentComplaintChannels = new List<AccidentComplaintChannel>
            {
                new AccidentComplaintChannel
                {
                    Id = 1,
                    Description = "Analista",
                },
                new AccidentComplaintChannel
                {
                    Id = 2,
                    Description = "Cuenta",
                }
            };

            var accidentComplaintChannelResponse = new List<AccidentComplaintChannelResponse>
            {
                new AccidentComplaintChannelResponse
                {
                    Id = accidentComplaintChannels[0].Id,
                    Description = accidentComplaintChannels[0].Description,
                },
                new AccidentComplaintChannelResponse
                {
                    Id = accidentComplaintChannels[1].Id,
                    Description = accidentComplaintChannels[1].Description,
                }
            };

            var mockAccidentComplaintChannelManager = _autoMoqer.GetMock<IAccidentComplaintChannelManager>();
            mockAccidentComplaintChannelManager.Setup(x => x.Get())
                .Returns(accidentComplaintChannels);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccidentComplaintChannelResponse>>(accidentComplaintChannels))
                .Returns(accidentComplaintChannelResponse);

            //ACT
            var result = _accidentComplaintChannelController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<AccidentComplaintChannelResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < accidentComplaintChannels.Count; i++)
            {
                Assert.Equal(value[i].Id, accidentComplaintChannelResponse[i].Id);
                Assert.Equal(value[i].Description, accidentComplaintChannelResponse[i].Description);
            }
        }
    }
}
