using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.AccidentNotificationDischargeChannel.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class AccidentNotificationDischargeChannelControllerTest
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly AccidentNotificationDischargeChannelController _accidentNotificationDischargeChannelController;

        public AccidentNotificationDischargeChannelControllerTest()
        {
            _autoMoqer = new AutoMoqer();
            _accidentNotificationDischargeChannelController = _autoMoqer.Resolve<AccidentNotificationDischargeChannelController>();
        }


        [Fact]
        public void Get()
        {
            //ARRANGE
            var accidentNotificationDischargeChannel = new List<AccidentNotificationDischargeChannel>
            {
                new AccidentNotificationDischargeChannel
                {
                    Id = 1,
                    Description = "Hospital",
                },
                new AccidentNotificationDischargeChannel
                {
                    Id = 2,
                    Description = "Consultorio",
                }
            };

            var accidentNotificationDischargeChannelResponse = new List<AccidentNotificationDischargeChannelResponse>
            {
                new AccidentNotificationDischargeChannelResponse
                {
                    Id = accidentNotificationDischargeChannel[0].Id,
                    Description = accidentNotificationDischargeChannel[0].Description,
                },
                new AccidentNotificationDischargeChannelResponse
                {
                    Id = accidentNotificationDischargeChannel[1].Id,
                    Description = accidentNotificationDischargeChannel[1].Description,
                }
            };

            var mockAccidentNotificationDischargeChannelManager = _autoMoqer.GetMock<IAccidentNotificationDischargeChannelManager>();
            mockAccidentNotificationDischargeChannelManager.Setup(x => x.Get())
                .Returns(accidentNotificationDischargeChannel);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccidentNotificationDischargeChannelResponse>>(accidentNotificationDischargeChannel))
                .Returns(accidentNotificationDischargeChannelResponse);

            //ACT
            var result = _accidentNotificationDischargeChannelController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<AccidentNotificationDischargeChannelResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < accidentNotificationDischargeChannel.Count; i++)
            {
                Assert.Equal(value[i].Id, accidentNotificationDischargeChannelResponse[i].Id);
                Assert.Equal(value[i].Description, accidentNotificationDischargeChannelResponse[i].Description);
            }
        }
    }
}
