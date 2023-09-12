using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.EmployeePathology.Request;
using VL.Health.Service.DTO.EmployeePathology.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
	public class EmployeePathologyControllerTests
	{
		private readonly AutoMoqer _autoMoqer;
		private readonly EmployeePathologyController _employeePathologyController;

		public EmployeePathologyControllerTests()
		{
			_autoMoqer = new AutoMoqer();
			_employeePathologyController = _autoMoqer.Resolve<EmployeePathologyController>();
		}

		[Fact]
		public void Get()
		{
			// Arrange
			int idEmployee = 1;
			var employeePathologys = new List<EmployeePathology>
			{
				new EmployeePathology
				{
					Id = 2
				},
				new EmployeePathology
				{
					Id = 3
				}
			};

			int pageNumber = 1;
			int pageSize = 50;
			var pagedList = PagedList<EmployeePathology>.ToPagedList(employeePathologys.AsQueryable(), pageNumber, pageSize);

			var response = new List<EmployeePathologyResponse>
			{
				new EmployeePathologyResponse
				{
					Id = 2
				},
				new EmployeePathologyResponse
				{
					Id = 3
				}
			};

			var pageFilter = new PageFilter();

			var employeePathologyManagerMock = _autoMoqer.GetMock<IEmployeePathologyManager>();
			employeePathologyManagerMock.Setup(x => x.Get(idEmployee, pageFilter)).Returns(pagedList);

			var mapperMock = _autoMoqer.GetMock<IMapper>();
			mapperMock.Setup(x => x.Map<List<EmployeePathology>, List<EmployeePathologyResponse>>(employeePathologys)).Returns(response);

			var mockHttpContextAccessor = _autoMoqer.GetMock<IHttpContextAccessor>();
			var context = new DefaultHttpContext();
			mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

			// Act
			var result = _employeePathologyController.Get(idEmployee, pageFilter);
			var model = (OkObjectResult)result.Result;
			var value = (List<EmployeePathologyResponse>)model.Value;

			// Assert
			Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);
			for (var i = 0; i < employeePathologys.Count; i++)
			{
				Assert.Equal(response[i].Id, value[i].Id);
			}
		}

		[Fact]
		public void Put()
		{
			// Arrange
			int idEmployee = 1;
			var employeePathologiesRequest = new EmployeePathologiesRequest()
			{
				IdEmployee = idEmployee,
				Pathologies = new List<EmployeePathologyRequest>()
				{
					new EmployeePathologyRequest() { Id = 2 },
					new EmployeePathologyRequest() { Id = 3 }
				}
			};

			// Act
			var result = _employeePathologyController.Put(employeePathologiesRequest);
			var model = (OkResult)result;

			// Assert
			Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);
		}
	}
}
