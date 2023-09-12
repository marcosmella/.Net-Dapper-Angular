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
    public class AccidentStatusManagerTest
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IAccidentStatusManager _accidentStatusManager;

        public AccidentStatusManagerTest()
        {
            _autoMoqer = new AutoMoqer();
            _accidentStatusManager = _autoMoqer.Resolve<AccidentStatusManager>();
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

            var mockAccidentStatusRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentStatusRepository.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentStatus>>>())).Returns(accidentStatuses);

            //ACT
            var result = _accidentStatusManager.Get();

            //ASSERT
            for (var i = 0; i < accidentStatuses.Count; i++)
            {
                Assert.Equal(result[i].Id, accidentStatuses[i].Id);
                Assert.Equal(result[i].Description, accidentStatuses[i].Description);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            var errorMessage = "Error";

            //ARRANGE
            var mockAccidentStatusValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentStatusValidator.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentStatus>>>()))
                .Throws(new FunctionalException(ErrorType.NotFound, errorMessage));

            //ACT
            try
            {
                var result = _accidentStatusManager.Get();
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
