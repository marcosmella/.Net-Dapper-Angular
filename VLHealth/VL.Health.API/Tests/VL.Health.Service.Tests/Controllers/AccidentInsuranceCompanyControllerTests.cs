using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.AccidentInsuranceCompany.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class AccidentInsuranceCompanyControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly AccidentInsuranceCompanyController _accidentInsuranceCompanyController;

        public AccidentInsuranceCompanyControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _accidentInsuranceCompanyController = _autoMoqer.Resolve<AccidentInsuranceCompanyController>();
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

            var accidentInsuranceCompanyResponse = new List<AccidentInsuranceCompanyResponse>
            {
                new AccidentInsuranceCompanyResponse
                {
                    Id = accidentInsuranceCompanies[0].Id,
                    Description = accidentInsuranceCompanies[0].Description,
                },
                new AccidentInsuranceCompanyResponse
                {
                    Id = accidentInsuranceCompanies[1].Id,
                    Description = accidentInsuranceCompanies[1].Description,
                }
            };

            var mockAccidentInsuranceCompanyManager = _autoMoqer.GetMock<IAccidentInsuranceCompanyManager>();
            mockAccidentInsuranceCompanyManager.Setup(x => x.Get())
                .Returns(accidentInsuranceCompanies);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AccidentInsuranceCompanyResponse>>(accidentInsuranceCompanies))
                .Returns(accidentInsuranceCompanyResponse);

            //ACT
            var result = _accidentInsuranceCompanyController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<AccidentInsuranceCompanyResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < accidentInsuranceCompanies.Count; i++)
            {
                Assert.Equal(value[i].Id, accidentInsuranceCompanyResponse[i].Id);
                Assert.Equal(value[i].Description, accidentInsuranceCompanyResponse[i].Description);
            }
        }
    }
}
