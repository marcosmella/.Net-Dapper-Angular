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

namespace VL.Health.API.Tests.Managers
{
    public class MedicalControlActionManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IMedicalControlActionManager _medicalControlActionManager;

        public MedicalControlActionManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _medicalControlActionManager = _autoMoqer.Resolve<MedicalControlActionManager>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var medicalControlActions = new List<MedicalControlAction>
            {
                new MedicalControlAction
                {
                    Id = 1,
                    Description = "AA",
                    CreateAbsence = true
                },
                new MedicalControlAction
                {
                    Id = 2,
                    Description = "BB",
                    CreateAbsence = false
                }
            };

            var mockMedicalControlActionRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockMedicalControlActionRepository.Setup(x => x.ListResult(It.IsAny<Func<List<MedicalControlAction>>>())).Returns(medicalControlActions);

            //ACT
            var result = _medicalControlActionManager.Get();

            //ASSERT
            for (var i = 0; i < medicalControlActions.Count; i++)
            {
                Assert.Equal(result[i].Id, medicalControlActions[i].Id);
                Assert.Equal(result[i].Description, medicalControlActions[i].Description);
                Assert.Equal(result[i].CreateAbsence, medicalControlActions[i].CreateAbsence);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            //ARRANGE
            var errorMessage = "Error";
            var mockMedicalControlActionRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockMedicalControlActionRepository.Setup(x => x.ListResult(It.IsAny<Func<List<MedicalControlAction>>>()))
                .Throws(new FunctionalException(ErrorType.NotFound, errorMessage));

            //ACT
            try
            {
                var result = _medicalControlActionManager.Get();
            }
            catch (Exception ex)
            {
                //ASSERT
                Assert.Equal(typeof(FunctionalException), ex.GetType());
                Assert.Equal(ErrorType.NotFound, ((FunctionalException)ex).FunctionalError);
            }
        }

        [Fact]
        public void GetByControlType()
        {
            //ARRANGE
            var medicalControlActions = new List<MedicalControlAction>
            {
                new MedicalControlAction
                {
                    Id = 1,
                    Description = "AA",
                    CreateAbsence = true
                },
                new MedicalControlAction
                {
                    Id = 2,
                    Description = "BB",
                    CreateAbsence = false
                }
            };

            int IdControlType = 1;
            var mockMedicalControlActionRepository = _autoMoqer.GetMock<IHelperResultValidator>();
            mockMedicalControlActionRepository.Setup(x => x.ListResult(It.IsAny<Func<int, List<MedicalControlAction>>>(), IdControlType)).Returns(medicalControlActions);

            //ACT
            var result = _medicalControlActionManager.GetByControlType(IdControlType);

            //ASSERT
            for (var i = 0; i < medicalControlActions.Count; i++)
            {
                Assert.Equal(result[i].Id, medicalControlActions[i].Id);
                Assert.Equal(result[i].Description, medicalControlActions[i].Description);
                Assert.Equal(result[i].CreateAbsence, medicalControlActions[i].CreateAbsence);
            }
        }


    }
}
