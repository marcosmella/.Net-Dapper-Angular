using AutoMoqCore;
using Moq;
using System;
using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.API.Managers;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.API.Helpers.Interfaces;
using Xunit;

namespace VL.Health.API.Tests.Managers
{
    public class AccidentMedicalProviderManagerTest
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IAccidentMedicalProviderManager _accidentMedicalProviderManager;

        public AccidentMedicalProviderManagerTest()
        {
            _autoMoqer = new AutoMoqer();
            _accidentMedicalProviderManager = _autoMoqer.Resolve<AccidentMedicalProviderManager>();
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
                    Description = "Brokers",
                },
                new AccidentMedicalProvider
                {
                    Id = 2,
                    Description = "ART",

                }
            };

            var mockAccidentMedicalProviderRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentMedicalProviderRepository.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentMedicalProvider>>>())).Returns(accidentMedicalProvider);

            //ACT
            var result = _accidentMedicalProviderManager.Get();

            //ASSERT
            for (var i = 0; i < accidentMedicalProvider.Count; i++)
            {
                Assert.Equal(result[i].Id, accidentMedicalProvider[i].Id);
                Assert.Equal(result[i].Description, accidentMedicalProvider[i].Description);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            var errorMessage = "Error";

            //ARRANGE
            var mockAccidentMedicalProviderRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentMedicalProviderRepository.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentMedicalProvider>>>()))
                .Throws(new FunctionalException(ErrorType.NotFound, errorMessage));

            //ACT
            try
            {
                var result = _accidentMedicalProviderManager.Get();
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
