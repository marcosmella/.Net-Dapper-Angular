using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.EmployeeMedicalExam.Request;
using VL.Health.Service.DTO.EmployeeMedicalExam.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class EmployeeMedicalExamControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly EmployeeMedicalExamController _employeeMedicalExamController;

        public EmployeeMedicalExamControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _employeeMedicalExamController = _autoMoqer.Resolve<EmployeeMedicalExamController>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var employeeMedicalExams = new List<EmployeeMedicalExam>
            {
                new EmployeeMedicalExam
                {
                    Id = 1,
                    IdEmployee = 16785,
                    IdFileType = 10001,
                    IdFile = 281,
                    ExpirationDate = DateTime.Now
                },
                new EmployeeMedicalExam
                {
                    Id = 2,
                    IdEmployee = 16784,
                    IdFileType = 10001,
                    IdFile = 282,
                    ExpirationDate = DateTime.Now
                }
            };

            var employeeMedicalExamsResponse = new List<EmployeeMedicalExamResponse>
            {
                new EmployeeMedicalExamResponse
                {
                    Id = employeeMedicalExams[0].Id,
                    IdEmployee = employeeMedicalExams[0].IdEmployee,
                    IdFileType = employeeMedicalExams[0].IdFileType,
                    IdFile = employeeMedicalExams[0].IdFile,
                    ExpirationDate = employeeMedicalExams[0].ExpirationDate
                },
                new EmployeeMedicalExamResponse
                {
                    Id = employeeMedicalExams[1].Id,
                    IdEmployee = employeeMedicalExams[1].IdEmployee,
                    IdFileType = employeeMedicalExams[1].IdFileType,
                    IdFile = employeeMedicalExams[1].IdFile,
                    ExpirationDate = employeeMedicalExams[1].ExpirationDate
                }
            };

            var mockEmployeeMedicalExamManager = _autoMoqer.GetMock<IEmployeeMedicalExamManager>();
            mockEmployeeMedicalExamManager.Setup(x => x.Get())
                .Returns(employeeMedicalExams);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<EmployeeMedicalExamResponse>>(employeeMedicalExams))
                .Returns(employeeMedicalExamsResponse);

            //ACT
            var result = _employeeMedicalExamController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<EmployeeMedicalExamResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < employeeMedicalExams.Count; i++)
            {
                Assert.Equal(value[i].Id, employeeMedicalExamsResponse[i].Id);
                Assert.Equal(value[i].IdEmployee, employeeMedicalExamsResponse[i].IdEmployee);
                Assert.Equal(value[i].IdFileType, employeeMedicalExamsResponse[i].IdFileType);
                Assert.Equal(value[i].IdFile, employeeMedicalExamsResponse[i].IdFile);
                Assert.Equal(value[i].ExpirationDate, employeeMedicalExamsResponse[i].ExpirationDate);
            }
        }

        [Fact]
        public void GetById()
        {
            //ARRANGE
            int idEmployeeMedicalExam = 5;
            var employeeMedicalExam = new EmployeeMedicalExam
            {
                Id = idEmployeeMedicalExam,
                IdEmployee = 16785,
                IdFileType = 10001,
                IdFile = 281,
                ExpirationDate = DateTime.Now
            };

            var employeeMedicalExamResponse = new EmployeeMedicalExamResponse
            {
                Id = employeeMedicalExam.Id,
                IdEmployee = employeeMedicalExam.IdEmployee,
                IdFileType = employeeMedicalExam.IdFileType,
                IdFile = employeeMedicalExam.IdFile,
                ExpirationDate = employeeMedicalExam.ExpirationDate
            };

            var mockEmployeeMedicalExamManager = _autoMoqer.GetMock<IEmployeeMedicalExamManager>();
            mockEmployeeMedicalExamManager.Setup(x => x.GetEmployeeMedicalExam(idEmployeeMedicalExam))
                .Returns(employeeMedicalExam);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<EmployeeMedicalExamResponse>(employeeMedicalExam))
                .Returns(employeeMedicalExamResponse);

            //ACT
            var result = _employeeMedicalExamController.Get(idEmployeeMedicalExam);
            var model = (OkObjectResult)result.Result;
            var value = (EmployeeMedicalExamResponse)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            Assert.Equal(value.Id, employeeMedicalExam.Id);
            Assert.Equal(value.IdEmployee, employeeMedicalExam.IdEmployee);
            Assert.Equal(value.IdFileType, employeeMedicalExam.IdFileType);
            Assert.Equal(value.IdFile, employeeMedicalExam.IdFile);
            Assert.Equal(value.ExpirationDate, employeeMedicalExam.ExpirationDate);
        }

        [Fact]
        public void Post()
        {
            //ARRANGE
            int idEmployeeMedicalExam = 2;

            var employeeMedicalExamRequest = new EmployeeMedicalExamRequest
            {
                Id = idEmployeeMedicalExam,
                IdEmployee = 16785,
                IdFileType = 10001,
                IdFile = 281,
                ExpirationDate = DateTime.Now
            };

            var employeeMedicalExam = new EmployeeMedicalExam
            {
                Id = employeeMedicalExamRequest.Id,
                IdEmployee = employeeMedicalExamRequest.IdEmployee,
                IdFileType = employeeMedicalExamRequest.IdFileType,
                IdFile = employeeMedicalExamRequest.IdFile,
                ExpirationDate = employeeMedicalExamRequest.ExpirationDate
            };

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<EmployeeMedicalExam>(employeeMedicalExamRequest))
                .Returns(employeeMedicalExam);

            var mockEmployeeMedicalExamManager = _autoMoqer.GetMock<IEmployeeMedicalExamManager>();
            mockEmployeeMedicalExamManager.Setup(x => x.Create(employeeMedicalExam))
                .Returns(idEmployeeMedicalExam);


            //ACT
            var result = _employeeMedicalExamController.Post(employeeMedicalExamRequest);
            var model = (OkObjectResult)result.Result;
            var response = (int)model.Value;

            //ASSERT
            Assert.Equal(model.StatusCode, (int)HttpStatusCode.OK);
            mockMapper.Verify(x => x.Map<EmployeeMedicalExam>(employeeMedicalExamRequest));
            mockEmployeeMedicalExamManager.Verify(x => x.Create(employeeMedicalExam));
            Assert.Equal(response, idEmployeeMedicalExam);
        }

        [Fact]
        public void Put()
        {
            //ARRANGE
            int idEmployeeMedicalExam = 2;

            var employeeMedicalExamRequest = new EmployeeMedicalExamRequest
            {
                Id = idEmployeeMedicalExam,
                IdEmployee = 16785,
                IdFileType = 10001,
                IdFile = 282,
                ExpirationDate = DateTime.Now
            };

            var employeeMedicalExam = new EmployeeMedicalExam
            {
                Id = employeeMedicalExamRequest.Id,
                IdEmployee = employeeMedicalExamRequest.IdEmployee,
                IdFileType = employeeMedicalExamRequest.IdFileType,
                IdFile = employeeMedicalExamRequest.IdFile,
                ExpirationDate = employeeMedicalExamRequest.ExpirationDate
            };

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<EmployeeMedicalExam>(employeeMedicalExamRequest))
                .Returns(employeeMedicalExam);

            var mockEmployeeMedicalExamManager = _autoMoqer.GetMock<IEmployeeMedicalExamManager>();
            mockEmployeeMedicalExamManager.Setup(x => x.Update(employeeMedicalExam));

            //ACT                      
            var result = _employeeMedicalExamController.Put(employeeMedicalExamRequest);
            var response = (OkResult)result;

            //ASSERT
            Assert.Equal(response.StatusCode, (int)HttpStatusCode.OK);
            mockMapper.Verify(x => x.Map<EmployeeMedicalExam>(employeeMedicalExamRequest));
            mockEmployeeMedicalExamManager.Verify(x => x.Update(employeeMedicalExam));
        }

        [Fact]
        public void Delete()
        {
            //ARRANGE
            var idEmployeeMedicalExam = 2;

            var mockEmployeeMedicalExamManager = _autoMoqer.GetMock<IEmployeeMedicalExamManager>();
            mockEmployeeMedicalExamManager.Setup(x => x.Delete(idEmployeeMedicalExam));


            //ACT
            var result = _employeeMedicalExamController.Delete(idEmployeeMedicalExam);
            var response = (OkResult)result;

            //ASSERT
            Assert.Equal(response.StatusCode, (int)HttpStatusCode.OK);
            mockEmployeeMedicalExamManager.Verify(x => x.Delete(idEmployeeMedicalExam));
        }
    }
}
