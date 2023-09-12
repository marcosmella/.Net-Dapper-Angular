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
using VL.Health.Service.DTO.EmployeeVaccine.Request;
using VL.Health.Service.DTO.EmployeeVaccine.Response;
using Xunit;

namespace VL.Health.Service.Tests.Controllers
{
    public class EmployeeVaccineControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly EmployeeVaccineController _employeeVaccineController;

        public EmployeeVaccineControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _employeeVaccineController = _autoMoqer.Resolve<EmployeeVaccineController>();
        }

		[Fact]
		public void Get()
		{
			// Arrange
			int idEmployee = 1;
			var employeeVaccines = new List<EmployeeVaccine>
			{
				new EmployeeVaccine
				{
					IdVaccine = 1,
					ApplicationDate = new DateTime(2021, 05, 6)
				},
				new EmployeeVaccine
				{
					IdVaccine = 1,
					ApplicationDate = new DateTime(2021, 07, 1)
				}
			};

			int pageNumber = 1;
			int pageSize = 50;
			var pagedList = PagedList<EmployeeVaccine>.ToPagedList(employeeVaccines.AsQueryable(), pageNumber, pageSize);

			var response = new List<EmployeeVaccineResponse>
			{
				new EmployeeVaccineResponse
				{
					IdVaccine = 1,
					ApplicationDate = new DateTime(2021, 05, 6)
				},
				new EmployeeVaccineResponse
				{
					IdVaccine = 1,
					ApplicationDate = new DateTime(2021, 07, 1)
				}
			};

			var pageFilter = new PageFilter();

			var employeeVaccineManagerMock = _autoMoqer.GetMock<IEmployeeVaccineManager>();
			employeeVaccineManagerMock.Setup(x => x.Get(idEmployee, pageFilter)).Returns(pagedList);

			var mapperMock = _autoMoqer.GetMock<IMapper>();
			mapperMock.Setup(x => x.Map<List<EmployeeVaccine>, List<EmployeeVaccineResponse>>(employeeVaccines)).Returns(response);

			var mockHttpContextAccessor = _autoMoqer.GetMock<IHttpContextAccessor>();
			var context = new DefaultHttpContext();
			mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

			// Act
			var result = _employeeVaccineController.Get(idEmployee, pageFilter);
			var model = (OkObjectResult)result.Result;
			var value = (List<EmployeeVaccineResponse>)model.Value;

			// Assert
			Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);
			for (var i = 0; i < employeeVaccines.Count; i++)
			{
				Assert.Equal(response[i].IdVaccine, value[i].IdVaccine);
				Assert.Equal(response[i].ApplicationDate, value[i].ApplicationDate);
			}
		}

		[Fact]
		public void Put()
		{
			// Arrange
			int idEmployee = 1;
			var employeeVaccinesRequest = new EmployeeVaccinesRequest()
			{
				IdEmployee = idEmployee,
				Vaccines = new List<EmployeeVaccineRequest>()
				{
					new EmployeeVaccineRequest() { IdVaccine = 8, ApplicationDate = DateTime.Now },
					new EmployeeVaccineRequest() { IdVaccine = 9, ApplicationDate = DateTime.Now }
				}
			};

			// Act
			var result = _employeeVaccineController.Put(employeeVaccinesRequest);
			var model = (OkResult)result;
			
			// Assert
			Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);
		}
	}
}
