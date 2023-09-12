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
using Xunit;

namespace VL.Health.API.Tests.Managers
{
    public class AccidentTypeManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IAccidentTypeManager _accidentTypeManager;

        public AccidentTypeManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _accidentTypeManager = _autoMoqer.Resolve<AccidentTypeManager>();
            
        }

        [Fact]
        public void Get()
        {
            //ARRANGE
            var accidentTypes = new List<AccidentType>
            {
                new AccidentType
                {
                    Id = 1,
                    Description = "Accidente In Itinere"
                },
                new AccidentType
                {
                    Id = 2,
                    Description = "Accidente de Trabajo"
                }
            };

            var mockHelperResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockHelperResultValidator.Setup(x => x.ListResult<AccidentType>(It.IsAny<Func<List<AccidentType>>>()))
                .Returns(accidentTypes);

            //ACT
            var result = _accidentTypeManager.Get();

            //ASSERT
            for (var i = 0; i < accidentTypes.Count; i++)
            {
                Assert.Equal(accidentTypes[i].Id, result[i].Id);
                Assert.Equal(accidentTypes[i].Description, result[i].Description);
                
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            //ARRANGE
            var accidentTypes = new List<AccidentType>();

            var mockHelperResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockHelperResultValidator.Setup(x => x.ListResult<AccidentType>(It.IsAny<Func<List<AccidentType>>>()))
                .Returns(accidentTypes);

            //ACT
            try
            {
                var result = _accidentTypeManager.Get();
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
