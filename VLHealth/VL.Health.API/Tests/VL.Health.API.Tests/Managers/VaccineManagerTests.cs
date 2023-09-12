using AutoMoqCore;
using System;
using System.Collections.Generic;
using System.Text;
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
    public class VaccineManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IVaccineManager _vaccineManager;

        public VaccineManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _vaccineManager = _autoMoqer.Resolve<VaccineManager>();
        }

        [Fact]
        public void Get()
        {
            // Arrange
            var vaccines = new List<Vaccine>
            {
                new Vaccine
                {
                    Id = 1,
                    Description = "COVID-19 - AstraZeneca"
                },
                new Vaccine
                {
                    Id = 2,
                    Description = "Hepatitis B"
                },
                new Vaccine
                {
                    Id = 3,
                    Description = "Triple Viral"
                }
            };

            var vaccineRepositoryMock = _autoMoqer.GetMock<IVaccineRepository>();

            var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
            helperResultValidatorMock.Setup(x => x.ListResult<Vaccine>(vaccineRepositoryMock.Object.Get)).Returns(vaccines);

            // Act
            var result = _vaccineManager.Get();

            // Assert
            for (var i = 0; i < vaccines.Count; i++)
            {
                Assert.Equal(vaccines[i].Id, result[i].Id);
                Assert.Equal(vaccines[i].Description, result[i].Description);
            }
        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            // Arranfe
            var vaccineRepositoryMock = _autoMoqer.GetMock<IVaccineRepository>();
            vaccineRepositoryMock.Setup(x => x.Get()).Returns(new List<Vaccine>());

            // Act
            try
            {
                var result = _vaccineManager.Get();
            }
            catch (Exception ex)
            {
                // Assert
                Assert.Equal(typeof(FunctionalException), ex.GetType());
                Assert.Equal(ErrorType.NotFound, ((FunctionalException)ex).FunctionalError);
            }
        }
    }
}
