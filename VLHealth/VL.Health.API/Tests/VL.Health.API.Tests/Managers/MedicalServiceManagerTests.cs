using AutoMoqCore;
using System;
using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.API.Managers;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using Xunit;
using VL.Health.API.Helpers.Interfaces;
using Moq;
using VL.Health.API.Validators.Interfaces;

namespace VL.Health.API.Tests.Managers
{
    public class MedicalServiceManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IMedicalServiceManager _medicalServiceManager;

        public MedicalServiceManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _medicalServiceManager = _autoMoqer.Resolve<MedicalServiceManager>();
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
                    Company = "ServicioMedico2",
                    Phone = "1234"
                }
            };

            var mockMedicalServiceRepositoryValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockMedicalServiceRepositoryValidator.Setup(x => x.ListResult(It.IsAny<Func<List<MedicalService>>>())).Returns(medicalServices);

            //ACT
            var result = _medicalServiceManager.Get();

            //ASSERT
            for (var i = 0; i < medicalServices.Count; i++)
            {
                Assert.Equal(result[i].Id, medicalServices[i].Id);
                Assert.Equal(result[i].Company, medicalServices[i].Company);
                Assert.Equal(result[i].Phone, medicalServices[i].Phone);
            }
        }


        [Fact]
        public void GetById()
        {
            //ARRANGE
            int idMedicalService = 3;
            var medicalService = new MedicalService
            {
                Id = 1,
                Company = "ServicioMedico1",
                Phone = "1234"
            };

            var mockMedicalServiceRepositoryValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockMedicalServiceRepositoryValidator.Setup(x => x.ObjectResult(It.IsAny<Func<int, MedicalService>>(), idMedicalService)).Returns(medicalService);

            //ACT
            var result = _medicalServiceManager.Get(idMedicalService);

            //ASSERT
            Assert.Equal(result.Id, medicalService.Id);
            Assert.Equal(result.Company, medicalService.Company);
            Assert.Equal(result.Phone, medicalService.Phone);
        }


        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            //ARRANGE

            var mockMedicalServiceRepositoryValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockMedicalServiceRepositoryValidator.Setup(x => x.ListResult(It.IsAny<Func<List<MedicalService>>>())).Throws(new FunctionalException(ErrorType.NotFound, "Error"));

            //ACT
            try
            {
                var result = _medicalServiceManager.Get();
            }
            catch (Exception ex)
            {
                //ASSERT
                Assert.Equal(typeof(FunctionalException), ex.GetType());
                Assert.Equal(ErrorType.NotFound, ((FunctionalException)ex).FunctionalError);
            }
        }


        [Fact]
        public void CreateMedicalService()
        {
            //ARRANGE
            int idMedicalService = 2;
            var medicalService = new MedicalService
            {
                Id = idMedicalService,
                Company = "Hospital ABC",
                Phone = "12345"
            };

            bool throwException = true;

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.IntegerResult(It.IsAny<Func<MedicalService, int>>(), medicalService, throwException)).Returns(idMedicalService);

            var mockValidator = _autoMoqer.GetMock<ICustomValidator<MedicalService>>();
            mockValidator.Setup(x => x.IsValid(medicalService, ActionType.Create)).Returns(true);

            //ACT
            var result = _medicalServiceManager.Create(medicalService);

            //ASSERT
            Assert.Equal(idMedicalService, result);
            mockRepositoryResultValidator.Verify(x => x.IntegerResult(It.IsAny<Func<MedicalService, int>>(), medicalService, throwException), Times.Once());
        }

        [Fact]
        public void UpdateMedicalService()
        {
            //ARRANGE
            int idMedicalService = 2;
            var medicalService = new MedicalService
            {
                Id = idMedicalService,
                Company = "Hospital ABC",
                Phone = "12345"
            };

            bool throwException = true;

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.IntegerResult(It.IsAny<Func<MedicalService, int>>(), medicalService, throwException)).Returns(idMedicalService);

            var mockValidator = _autoMoqer.GetMock<ICustomValidator<MedicalService>>();
            mockValidator.Setup(x => x.IsValid(medicalService, ActionType.Update)).Returns(true);

            //ACT
            _medicalServiceManager.Update(medicalService);

            //ASSERT
            mockRepositoryResultValidator.Verify(x => x.IntegerResult(It.IsAny<Func<MedicalService, int>>(), medicalService, throwException), Times.Once());
        }

        [Fact]
        public void DeleteMedicalService()
        {
            //ARRANGE
            var idMedicalService = 2;
            var medicalService = new MedicalService() { Id = idMedicalService };

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.ObjectResult(It.IsAny<Func<int, MedicalService>>(), idMedicalService)).Returns(medicalService);

            //ACT
            _medicalServiceManager.Delete(idMedicalService);

            //ASSERT
            mockRepositoryResultValidator.Verify(x => x.ObjectResult(It.IsAny<Func<int, MedicalService>>(), idMedicalService), Times.Once());
        }


    }
}
