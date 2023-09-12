using AutoMoqCore;
using System;
using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.API.Managers;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;
using Xunit;

namespace VL.Health.API.Tests.Managers
{
    public class MedicalControlTypeManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IMedicalControlTypeManager _medicalControlTypeManager;

        public MedicalControlTypeManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _medicalControlTypeManager = _autoMoqer.Resolve<MedicalControlTypeManager>();
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

            var mockMedicalControlTypeRepository = _autoMoqer.GetMock<IMedicalControlTypeRepository>();
            mockMedicalControlTypeRepository.Setup(x => x.Get())
                .Returns(medicalControlTypes);

            //ACT
            var result = _medicalControlTypeManager.Get();

            //ASSERT
            for (var i = 0; i < medicalControlTypes.Count; i++)
            {
                Assert.Equal(result[i].Id, medicalControlTypes[i].Id);
                Assert.Equal(result[i].Description, medicalControlTypes[i].Description);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            //ARRANGE
            var mockMedicalControlTypeRepository = _autoMoqer.GetMock<IMedicalControlTypeRepository>();
            mockMedicalControlTypeRepository.Setup(x => x.Get())
                .Returns(new List<MedicalControlType>());

            //ACT
            try
            {
                var result = _medicalControlTypeManager.Get();
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
