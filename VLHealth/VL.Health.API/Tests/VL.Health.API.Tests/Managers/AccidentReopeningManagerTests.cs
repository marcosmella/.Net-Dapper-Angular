using AutoMoqCore;
using Moq;
using System;
using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.API.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Managers;
using VL.Health.Interfaces.Repositories;
using Xunit;

namespace VL.Health.API.Tests.Managers
{
    public class AccidentReopeningManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IAccidentReopeningManager _accidentReopeningManager;

        public AccidentReopeningManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _accidentReopeningManager = _autoMoqer.Resolve<AccidentReopeningManager>();
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var accidentReopenings = new List<AccidentReopening>
            {
                new AccidentReopening
                {
                    Id = 1,
                    Description = "Espontanea"
                },
                new AccidentReopening
                {
                    Id = 2,
                    Description = "Servicio Medico"
                }
            };

            var mockHelperResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockHelperResultValidator.Setup(x => x.ListResult<AccidentReopening>(It.IsAny<Func<List<AccidentReopening>>>()))
                .Returns(accidentReopenings);

            //ACT
            var result = _accidentReopeningManager.Get();

            //ASSERT
            for (var i = 0; i < accidentReopenings.Count; i++)
            {
                Assert.Equal(accidentReopenings[i].Id, result[i].Id);
                Assert.Equal(accidentReopenings[i].Description, result[i].Description);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            //ARRANGE
            var mockAccidentReopeningRepository = _autoMoqer.GetMock<IAccidentReopeningRepository>();
            mockAccidentReopeningRepository.Setup(x => x.Get())
                .Returns(new List<AccidentReopening>());

            //ACT
            try
            {
                var result = _accidentReopeningManager.Get();
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
