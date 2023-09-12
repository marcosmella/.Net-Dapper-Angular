using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.Pathology.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class PathologyControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly PathologyController _pathologyController;

        public PathologyControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _pathologyController = _autoMoqer.Resolve<PathologyController>();
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

            var pathologiesResponse = new List<PathologyResponse>
            {
                new PathologyResponse
                {
                    Id = pathologies[0].Id,
                    Description = pathologies[0].Description
                },
                new PathologyResponse
                {
                    Id = pathologies[1].Id,
                    Description = pathologies[1].Description
                }
            };            

            var mockPathologyManager = _autoMoqer.GetMock<IPathologyManager>();
            mockPathologyManager.Setup(x => x.Get(""))
                .Returns(pathologies);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<PathologyResponse>>(pathologies))
                .Returns(pathologiesResponse);

            //ACT
            var result = _pathologyController.Get("");
            var model = (OkObjectResult)result.Result;
            var value = (List<PathologyResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < pathologies.Count; i++)
            {
                Assert.Equal(value[i].Id, pathologiesResponse[i].Id);
                Assert.Equal(value[i].Description, pathologiesResponse[i].Description);               
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

            var pathologyResponse = new PathologyResponse
            {
                Id = pathology.Id,
                Description = "Anemia"
            };

            var mockPathologyManager = _autoMoqer.GetMock<IPathologyManager>();
            mockPathologyManager.Setup(x => x.GetById(idPathology))
                .Returns(pathology);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<PathologyResponse>(pathology))
                .Returns(pathologyResponse);

            //ACT
            var result = _pathologyController.GetById(idPathology);
            var model = (OkObjectResult)result.Result;
            var value = (PathologyResponse)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            Assert.Equal(value.Id, pathology.Id);
            Assert.Equal(value.Description, pathology.Description);
        }
    }
}
