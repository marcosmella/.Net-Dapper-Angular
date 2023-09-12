using AutoMoqCore;
using Moq;
using System;
using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.API.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.API.Helpers.Interfaces;
using Xunit;
using VL.Health.Interfaces.Managers;

namespace VL.Health.API.Tests.Managers
{
    public class AccidentComplaintChannelManagerTest
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IAccidentComplaintChannelManager _accidentComplaintChannelManager;

        public AccidentComplaintChannelManagerTest()
        {
            _autoMoqer = new AutoMoqer();
            _accidentComplaintChannelManager = _autoMoqer.Resolve<AccidentComplaintChannelManager>();
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
                    Description = "Craneo",
                },
                new AccidentComplaintChannel
                {
                    Id = 2,
                    Description = "Cervical",

                }
            };

            var mockAccidentComplaintChannelRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentComplaintChannelRepository.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentComplaintChannel>>>())).Returns(accidentComplaintChannels);

            //ACT
            var result = _accidentComplaintChannelManager.Get();

            //ASSERT
            for (var i = 0; i < accidentComplaintChannels.Count; i++)
            {
                Assert.Equal(result[i].Id, accidentComplaintChannels[i].Id);
                Assert.Equal(result[i].Description, accidentComplaintChannels[i].Description);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            var errorMessage = "Error";

            //ARRANGE
            var mockAccidentComplaintChannelRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentComplaintChannelRepository.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentComplaintChannel>>>()))
                .Throws(new FunctionalException(ErrorType.NotFound, errorMessage));

            //ACT
            try
            {
                var result = _accidentComplaintChannelManager.Get();
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
