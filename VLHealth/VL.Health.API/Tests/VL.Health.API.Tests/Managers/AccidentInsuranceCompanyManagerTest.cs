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
    public class AccidentInsuranceCompanyManagerTest
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IAccidentInsuranceCompanyManager _accidentInsuranceCompanyManager;

        public AccidentInsuranceCompanyManagerTest()
        {
            _autoMoqer = new AutoMoqer();
            _accidentInsuranceCompanyManager = _autoMoqer.Resolve<AccidentInsuranceCompanyManager>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var accidentInsuranceCompanies = new List<AccidentInsuranceCompany>
            {
                new AccidentInsuranceCompany
                {
                    Id = 1,
                    Description = "AA",
                },
                new AccidentInsuranceCompany
                {
                    Id = 2,
                    Description = "BB",

                }
            };

            var mockAccidentInsuranceCompanyRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentInsuranceCompanyRepository.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentInsuranceCompany>>>())).Returns(accidentInsuranceCompanies);

            //ACT
            var result = _accidentInsuranceCompanyManager.Get();

            //ASSERT
            for (var i = 0; i < accidentInsuranceCompanies.Count; i++)
            {
                Assert.Equal(result[i].Id, accidentInsuranceCompanies[i].Id);
                Assert.Equal(result[i].Description, accidentInsuranceCompanies[i].Description);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            var errorMessage = "Error";

            //ARRANGE
            var mockAccidentInsuranceCompanyRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockAccidentInsuranceCompanyRepository.Setup(x => x.ListResult(It.IsAny<Func<List<AccidentInsuranceCompany>>>()))
                .Throws(new FunctionalException(ErrorType.NotFound, errorMessage));

            //ACT
            try
            {
                var result = _accidentInsuranceCompanyManager.Get();
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
