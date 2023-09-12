using AutoMoqCore;
using Moq;
using System;
using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.API.Managers;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers.Interfaces;
using Xunit;


namespace VL.Health.API.Tests.Managers
{
    public class AccidentPathologyManagerTest
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IAccidentPathologyManager _accidentPathologyManager;

        public AccidentPathologyManagerTest()
        {
            _autoMoqer = new AutoMoqer();
            _accidentPathologyManager = _autoMoqer.Resolve<AccidentPathologyManager>();
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
                    Description = "Craneo",
                },
                new AccidentPathology
                {
                    Id = 2,
                    Description = "Cervical",

                }
            };

            var mockAccidentPathologyRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentPathologyRepository.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentPathology>>>())).Returns(accidentPathology);

            //ACT
            var result = _accidentPathologyManager.Get();

            //ASSERT
            for (var i = 0; i < accidentPathology.Count; i++)
            {
                Assert.Equal(result[i].Id, accidentPathology[i].Id);
                Assert.Equal(result[i].Description, accidentPathology[i].Description);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            var errorMessage = "Error";

            //ARRANGE
            var mockAccidentPathologyRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentPathologyRepository.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentPathology>>>()))
                .Throws(new FunctionalException(ErrorType.NotFound, errorMessage));

            //ACT
            try
            {
                var result = _accidentPathologyManager.Get();
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
