using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.MedicalControlAction.Response;
using Xunit;


namespace VL.Health.Service.Tests.Controllers
{
    public class MedicalControlActionControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly MedicalControlActionController _medicalControlActionController;

        public MedicalControlActionControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _medicalControlActionController = _autoMoqer.Resolve<MedicalControlActionController>();
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

            var medicalControlActionsResponse = new List<MedicalControlActionResponse>
            {
                new MedicalControlActionResponse
                {
                    Id = medicalControlActions[0].Id,
                    Description = medicalControlActions[0].Description,
                    CreateAbsence = medicalControlActions[0].CreateAbsence
                },
                new MedicalControlActionResponse
                {
                    Id = medicalControlActions[1].Id,
                    Description = medicalControlActions[1].Description,
                    CreateAbsence = medicalControlActions[1].CreateAbsence
                }
            };

            var mockMedicalControlActionManager = _autoMoqer.GetMock<IMedicalControlActionManager>();
            mockMedicalControlActionManager.Setup(x => x.Get())
                .Returns(medicalControlActions);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MedicalControlActionResponse>>(medicalControlActions))
                .Returns(medicalControlActionsResponse);

            //ACT
            var result = _medicalControlActionController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<MedicalControlActionResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < medicalControlActions.Count; i++)
            {
                Assert.Equal(value[i].Id, medicalControlActionsResponse[i].Id);
                Assert.Equal(value[i].Description, medicalControlActionsResponse[i].Description);
                Assert.Equal(value[i].CreateAbsence, medicalControlActionsResponse[i].CreateAbsence);
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

            var medicalControlActionsResponse = new List<MedicalControlActionResponse>
            {
                new MedicalControlActionResponse
                {
                    Id = medicalControlActions[0].Id,
                    Description = medicalControlActions[0].Description,
                    CreateAbsence = medicalControlActions[0].CreateAbsence
                },
                new MedicalControlActionResponse
                {
                    Id = medicalControlActions[1].Id,
                    Description = medicalControlActions[1].Description,
                    CreateAbsence = medicalControlActions[1].CreateAbsence
                }
            };

            int idControlType = 1;
            var mockMedicalControlActionManager = _autoMoqer.GetMock<IMedicalControlActionManager>();
            mockMedicalControlActionManager.Setup(x => x.GetByControlType(idControlType))
                .Returns(medicalControlActions);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MedicalControlActionResponse>>(medicalControlActions))
                .Returns(medicalControlActionsResponse);

            //ACT
            var result = _medicalControlActionController.GetByControlType(idControlType);
            var model = (OkObjectResult)result.Result;
            var value = (List<MedicalControlActionResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < medicalControlActions.Count; i++)
            {
                Assert.Equal(value[i].Id, medicalControlActionsResponse[i].Id);
                Assert.Equal(value[i].Description, medicalControlActionsResponse[i].Description);
                Assert.Equal(value[i].CreateAbsence, medicalControlActionsResponse[i].CreateAbsence);
            }
        }



    }
}
