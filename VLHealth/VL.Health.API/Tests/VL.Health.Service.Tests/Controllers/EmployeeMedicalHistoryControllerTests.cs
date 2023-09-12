using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using VL.Audit.Client.Interface;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.EmployeeMedicalHistory.Request;
using VL.Health.Service.DTO.EmployeeMedicalHistory.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class EmployeeMedicalHistoryControllerTests
	{
        private readonly AutoMoqer _autoMoqer;
        private readonly EmployeeMedicalHistoryController _medicalHistoryController;

        public EmployeeMedicalHistoryControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _medicalHistoryController = _autoMoqer.Resolve<EmployeeMedicalHistoryController>();
        }

        [Fact]
        public void Get()
        {
            int idPerson = 647;
            int idBloodPressure = 2;
            int idBloodType = 3;
            bool isRiskGroup = false;

            var medicalHistory = new EmployeeMedicalHistory()
            {
                IdPerson = idPerson,
                IdBloodPressure = idBloodPressure,
                IdBloodType = idBloodType,
                IsRiskGroup = isRiskGroup
            };
            var medicalHistoryResponse = new EmployeeMedicalHistoryResponse()
            {
                IdPerson = idPerson,
                IdBloodPressure = idBloodPressure,
                IdBloodType = idBloodType,
                IsRiskGroup = isRiskGroup
            };

            var mapperMock = _autoMoqer.GetMock<IMapper>();
            mapperMock.Setup(x => x.Map<EmployeeMedicalHistory, EmployeeMedicalHistoryResponse>(medicalHistory)).Returns(medicalHistoryResponse);

            var medicalHistoryManagerMock = _autoMoqer.GetMock<IEmployeeMedicalHistoryManager>();
            medicalHistoryManagerMock.Setup(x => x.Get(idPerson)).Returns(medicalHistory);

            // Act
            var result = _medicalHistoryController.Get(idPerson);

            var model = (OkObjectResult)result.Result;
            var value = (EmployeeMedicalHistoryResponse)model.Value;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);
            Assert.Equal(medicalHistoryResponse, value);

            medicalHistoryManagerMock.Verify(x => x.Get(idPerson), Times.Once);
        }

        [Fact]
        public void Create()
        {
            int id = 1;

            int idPerson = 647;
            int idBloodPressure = 2;
			int idBloodType = 3;
            bool isRiskGroup = false;

            var medicalHistoryRequest = new EmployeeMedicalHistoryRequest()
            {
                IdPerson = idPerson,
                IdBloodPressure = idBloodPressure,
                IdBloodType = idBloodType,
                IsRiskGroup = isRiskGroup
            };
            var medicalHistory = new EmployeeMedicalHistory()
            {
                IdPerson = idPerson,
                IdBloodPressure = idBloodPressure,
                IdBloodType = idBloodType,
                IsRiskGroup = isRiskGroup
            };

            var mapperMock = _autoMoqer.GetMock<IMapper>();
            mapperMock.Setup(x => x.Map<EmployeeMedicalHistoryRequest, EmployeeMedicalHistory>(medicalHistoryRequest)).Returns(medicalHistory);

            var medicalHistoryManagerMock = _autoMoqer.GetMock<IEmployeeMedicalHistoryManager>();
            medicalHistoryManagerMock.Setup(x => x.Create(medicalHistory)).Returns(id);

            var auditClientMock = _autoMoqer.GetMock<IAuditClient>();
            auditClientMock.Setup(x => x.Save(medicalHistory, id, idPerson, (int)EntityType.Employee));

            // Act
            var result = _medicalHistoryController.Create(medicalHistoryRequest);

            var model = (OkObjectResult)result.Result;
            var value = (int)model.Value;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);
            Assert.Equal(id, value);

            medicalHistoryManagerMock.Verify(x => x.Create(medicalHistory), Times.Once);
            auditClientMock.Verify(x => x.Save(medicalHistory, id, idPerson, (int)EntityType.Employee), Times.Once);
        }

        [Fact]
        public void Put()
        {
            int id = 1;

            int idPerson = 647;
            int idBloodPressure = 2;
            int idBloodType = 3;
            bool isRiskGroup = false;

            var medicalHistoryRequest = new EmployeeMedicalHistoryRequest()
            {
                Id = id,
                IdPerson = idPerson,
                IdBloodPressure = idBloodPressure,
                IdBloodType = idBloodType,
                IsRiskGroup = isRiskGroup
            };
            var medicalHistory = new EmployeeMedicalHistory()
            {
                Id = id,
                IdPerson = idPerson,
                IdBloodPressure = idBloodPressure,
                IdBloodType = idBloodType,
                IsRiskGroup = isRiskGroup
            };

            var mapperMock = _autoMoqer.GetMock<IMapper>();
            mapperMock.Setup(x => x.Map<EmployeeMedicalHistoryRequest, EmployeeMedicalHistory>(medicalHistoryRequest)).Returns(medicalHistory);

            var medicalHistoryManagerMock = _autoMoqer.GetMock<IEmployeeMedicalHistoryManager>();
            medicalHistoryManagerMock.Setup(x => x.Update(medicalHistory));

            var auditClientMock = _autoMoqer.GetMock<IAuditClient>();
            auditClientMock.Setup(x => x.Save(medicalHistory, id, idPerson, (int)EntityType.Employee));

            // Act
            var result = _medicalHistoryController.Update(medicalHistoryRequest);

            var model = (OkResult)result;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            medicalHistoryManagerMock.Verify(x => x.Update(medicalHistory), Times.Once);
            auditClientMock.Verify(x => x.Save(medicalHistory, id, idPerson, (int)EntityType.Employee), Times.Once);
        }
    }
}
