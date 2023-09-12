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
    public class AccidentComplainantManagerTest
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IAccidentComplainantManager _accidentComplainantManager;

        public AccidentComplainantManagerTest()
        {
            _autoMoqer = new AutoMoqer();
            _accidentComplainantManager = _autoMoqer.Resolve<AccidentComplainantManager>();
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
                    Description = "Craneo",
                },
                new AccidentComplainant
                {
                    Id = 2,
                    Description = "Cervical",

                }
            };

            var mockAccidentComplainantRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentComplainantRepository.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentComplainant>>>())).Returns(accidentComplainants);

            //ACT
            var result = _accidentComplainantManager.Get();

            //ASSERT
            for (var i = 0; i < accidentComplainants.Count; i++)
            {
                Assert.Equal(result[i].Id, accidentComplainants[i].Id);
                Assert.Equal(result[i].Description, accidentComplainants[i].Description);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            var errorMessage = "Error";

            //ARRANGE
            var mockAccidentComplainantRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentComplainantRepository.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentComplainant>>>()))
                .Throws(new FunctionalException(ErrorType.NotFound, errorMessage));

            //ACT
            try
            {
                var result = _accidentComplainantManager.Get();
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
