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
using VL.Health.API.Helpers.Interfaces;
using Moq;

namespace VL.Health.API.Tests.Managers
{
    public class PathologyManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IPathologyManager _pathologyManager;

        public PathologyManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _pathologyManager = _autoMoqer.Resolve<PathologyManager>();
        } 

        [Fact]
        public void Get()
        {
            //ARRANGE
            var pathologies = new List<Pathology>
            {
                new Pathology
                {
                    Id = 1,
                    Description = "Dolor de Cabeza"                   
                },
                new Pathology
                {
                    Id = 2,
                    Description = "Gastritis"
                }
            };

            var mockPathologyRepository = _autoMoqer.GetMock<IPathologyRepository>();
            mockPathologyRepository.Setup(x => x.Get(""))
                .Returns(pathologies);

            //ACT
            var result = _pathologyManager.Get("");

            //ASSERT
            for (var i = 0; i < pathologies.Count; i++)
            {
                Assert.Equal(result[i].Id, pathologies[i].Id);
                Assert.Equal(result[i].Description, pathologies[i].Description);              
            }
        }

         [Fact]
        public void GetById()
        {
            //ARRANGE
            int idPathology = 5;
            var pathology = new Pathology
            {
                Id = idPathology,
                Description = "Anemia"
            };

            var mockPathologyRepositoryValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockPathologyRepositoryValidator.Setup(x => x.ObjectResult(It.IsAny<Func<int, Pathology>>(), idPathology)).Returns(pathology);

            //ACT
            var result = _pathologyManager.GetById(idPathology);

            //ASSERT
            Assert.Equal(result.Id, pathology.Id);
            Assert.Equal(result.Description, pathology.Description);

        }

        [Fact]
        public void GetMustThrowFunctionalExceptionNotFound()
        {
            //ARRANGE
            var mockPathologyRepository = _autoMoqer.GetMock<IPathologyRepository>();
            mockPathologyRepository.Setup(x => x.Get(""))
                .Returns(new List<Pathology>());

            //ACT
            try
            {
                var result = _pathologyManager.Get("");
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
