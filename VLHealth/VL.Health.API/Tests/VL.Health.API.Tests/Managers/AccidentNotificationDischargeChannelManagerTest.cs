using AutoMoqCore;
using Moq;
using System;
using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.API.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Managers;
using VL.Health.API.Helpers.Interfaces;
using Xunit;

namespace VL.Health.API.Tests.Managers
{
    public class AccidentNotificationDischargeChannelManagerTest
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IAccidentNotificationDischargeChannelManager _accidentNotificationDischargeChannelManager;

        public AccidentNotificationDischargeChannelManagerTest()
        {
            _autoMoqer = new AutoMoqer();
            _accidentNotificationDischargeChannelManager = _autoMoqer.Resolve<AccidentNotificationDischargeChannelManager>();
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
                    Description = "Consultorio",
                },
                new AccidentNotificationDischargeChannel
                {
                    Id = 2,
                    Description = "Hospital",

                }
            };

            var mockAccidentNotificationDischargeChannelRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentNotificationDischargeChannelRepository.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentNotificationDischargeChannel>>>())).Returns(accidentNotificationDischargeChannel);

            //ACT
            var result = _accidentNotificationDischargeChannelManager.Get();

            //ASSERT
            for (var i = 0; i < accidentNotificationDischargeChannel.Count; i++)
            {
                Assert.Equal(result[i].Id, accidentNotificationDischargeChannel[i].Id);
                Assert.Equal(result[i].Description, accidentNotificationDischargeChannel[i].Description);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            var errorMessage = "Error";

            //ARRANGE
            var mockAccidentNotificationDischargeChannelRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentNotificationDischargeChannelRepository.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentNotificationDischargeChannel>>>()))
                .Throws(new FunctionalException(ErrorType.NotFound, errorMessage));

            //ACT
            try
            {
                var result = _accidentNotificationDischargeChannelManager.Get();
            }
            catch (Exception ex)
            {
                //ASSERT
                Assert.Equal(typeof(FunctionalException), ex.GetType());
                Assert.Equal(ErrorType.NotFound, ((FunctionalException)ex).FunctionalError);
            }
        }
    }
}
