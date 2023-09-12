using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.MedicalService.Response;
using Xunit;
using VL.Health.Service.DTO.MedicalService.Request;

namespace VL.Health.Service.Tests.Controllers
{
    public class MedicalServiceControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly MedicalServiceController _medicalServiceController;

        public MedicalServiceControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _medicalServiceController = _autoMoqer.Resolve<MedicalServiceController>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var medicalServices = new List<MedicalService>
            {
                new MedicalService
                {
                    Id = 1,
                    Company = "ServicioMedico1",
                    Phone = "1234"
                },
                new MedicalService
                {
                    Id = 2,
                    Company = "ServicioMedico1",
                    Phone = "5678"
                }
            };

            var medicalServicesResponse = new List<MedicalServiceResponse>
            {
                new MedicalServiceResponse
                {
                    Id = medicalServices[0].Id,
                    Company = medicalServices[0].Company,
                    Phone = medicalServices[0].Phone
                },
                new MedicalServiceResponse
                {
                    Id = medicalServices[1].Id,
                    Company = medicalServices[1].Company,
                    Phone = medicalServices[1].Phone
                }
            };

            var mockMedicalServiceManager = _autoMoqer.GetMock<IMedicalServiceManager>();
            mockMedicalServiceManager.Setup(x => x.Get())
                .Returns(medicalServices);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MedicalServiceResponse>>(medicalServices))
                .Returns(medicalServicesResponse);

            //ACT
            var result = _medicalServiceController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<MedicalServiceResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < medicalServices.Count; i++)
            {
                Assert.Equal(value[i].Id, medicalServicesResponse[i].Id);
                Assert.Equal(value[i].Company, medicalServicesResponse[i].Company);
                Assert.Equal(value[i].Phone, medicalServicesResponse[i].Phone);
            }
        }


        [Fact]
        public void GetById()
        {
            //ARRANGE
            int idMedicalService = 3;
            var medicalService = new MedicalService
            {
                Id = idMedicalService,
                Company = "ABC",
                Phone = "223322431"
            };

            var medicalServiceResponse = new MedicalServiceResponse
            {
                Id = medicalService.Id,
                Company = medicalService.Company,
                Phone = medicalService.Phone
            };

            var mockMedicalServiceManager = _autoMoqer.GetMock<IMedicalServiceManager>();
            mockMedicalServiceManager.Setup(x => x.Get(idMedicalService))
                .Returns(medicalService);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<MedicalServiceResponse>(medicalService))
                .Returns(medicalServiceResponse);


            //ACT
            var result = _medicalServiceController.Get(idMedicalService);
            var model = (OkObjectResult)result.Result;
            var value = (MedicalServiceResponse)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            Assert.Equal(value.Id, medicalService.Id);
            Assert.Equal(value.Company, medicalService.Company);
            Assert.Equal(value.Phone, medicalService.Phone);
        }


        [Fact]
        public void Post()
        {
            //ARRANGE
            int idMedicalService = 2;

            var medicalServiceRequest = new MedicalServiceRequest
            {
                Id = idMedicalService,
                Company = "Hospital de Clinicas",
                Phone = "123456"
            };

            var medicalService = new MedicalService
            {
                Id = medicalServiceRequest.Id,
                Company = medicalServiceRequest.Company,
                Phone = medicalServiceRequest.Phone
            };

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<MedicalService>(medicalServiceRequest))
                .Returns(medicalService);

            var mockMedicalServiceManager = _autoMoqer.GetMock<IMedicalServiceManager>();
            mockMedicalServiceManager.Setup(x => x.Create(medicalService))
                .Returns(idMedicalService);

            //ACT
            var result = _medicalServiceController.Post(medicalServiceRequest);
            var model = (OkObjectResult)result.Result;
            var response = (int)model.Value;

            //ASSERT
            Assert.Equal(model.StatusCode, (int)HttpStatusCode.OK);
            mockMapper.Verify(x => x.Map<MedicalService>(medicalServiceRequest));
            mockMedicalServiceManager.Verify(x => x.Create(medicalService));
            Assert.Equal(response, idMedicalService);
        }

        [Fact]
        public void Put()
        {
            //ARRANGE
            int idMedicalService = 2;

            var medicalServiceRequest = new MedicalServiceRequest
            {
                Id = idMedicalService,
                Company = "Hospital de Clinicas",
                Phone = "123456"
            };

            var medicalService = new MedicalService
            {
                Id = medicalServiceRequest.Id,
                Company = medicalServiceRequest.Company,
                Phone = medicalServiceRequest.Phone
            };

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<MedicalService>(medicalServiceRequest))
                .Returns(medicalService);

            var mockMedicalServiceManager = _autoMoqer.GetMock<IMedicalServiceManager>();
            mockMedicalServiceManager.Setup(x => x.Update(medicalService));

            //ACT                      
            var result = _medicalServiceController.Put(medicalServiceRequest);
            var response = (OkResult)result;

            //ASSERT
            Assert.Equal(response.StatusCode, (int)HttpStatusCode.OK);
            mockMapper.Verify(x => x.Map<MedicalService>(medicalServiceRequest));
            mockMedicalServiceManager.Verify(x => x.Update(medicalService));
        }

        [Fact]
        public void Delete()
        {
            //ARRANGE
            var idMedicalService = 2;

            var mockMedicalServiceManager = _autoMoqer.GetMock<IMedicalServiceManager>();
            mockMedicalServiceManager.Setup(x => x.Delete(idMedicalService));


            //ACT
            var result = _medicalServiceController.Delete(idMedicalService);
            var response = (OkResult)result;

            //ASSERT
            Assert.Equal(response.StatusCode, (int)HttpStatusCode.OK);
            mockMedicalServiceManager.Verify(x => x.Delete(idMedicalService));
        }



    }
}
