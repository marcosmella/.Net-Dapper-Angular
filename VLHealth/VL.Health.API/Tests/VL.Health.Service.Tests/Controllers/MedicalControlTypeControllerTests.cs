using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.MedicalControlType.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class MedicalControlTypeControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly MedicalControlTypeController _medicalControlTypeController;

        public MedicalControlTypeControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _medicalControlTypeController = _autoMoqer.Resolve<MedicalControlTypeController>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var medicalControlTypes = new List<MedicalControlType>
            {
                new MedicalControlType
                {
                    Id = 1,
                    Description = "En consultorio"
                },
                new MedicalControlType
                {
                    Id = 2,
                    Description = "En domicilio"
                }
            };

            var medicalControlTypesResponse = new List<MedicalControlTypeResponse>
            {
                new MedicalControlTypeResponse
                {
                    Id = medicalControlTypes[0].Id,
                    Description = medicalControlTypes[0].Description
                },
                new MedicalControlTypeResponse
                {
                    Id = medicalControlTypes[1].Id,
                    Description = medicalControlTypes[1].Description
                }
            };

            var mockMedicalControlTypeManager = _autoMoqer.GetMock<IMedicalControlTypeManager>();
            mockMedicalControlTypeManager.Setup(x => x.Get())
                .Returns(medicalControlTypes);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MedicalControlTypeResponse>>(medicalControlTypes))
                .Returns(medicalControlTypesResponse);

            //ACT
            var result = _medicalControlTypeController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<MedicalControlTypeResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < medicalControlTypes.Count; i++)
            {
                Assert.Equal(value[i].Id, medicalControlTypesResponse[i].Id);
                Assert.Equal(value[i].Description, medicalControlTypesResponse[i].Description);
            }
        }
    }
}
