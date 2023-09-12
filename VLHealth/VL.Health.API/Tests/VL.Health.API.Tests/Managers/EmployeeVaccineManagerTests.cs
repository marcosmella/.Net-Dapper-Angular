using AutoMoqCore;
using Moq;
using System;
using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.API.Managers;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Managers;
using Xunit;

namespace VL.Health.API.Tests.Managers
{
    public class EmployeeVaccineManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IEmployeeVaccineManager _employeeVaccineManager;

        public EmployeeVaccineManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _employeeVaccineManager = _autoMoqer.Resolve<EmployeeVaccineManager>();
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
					ApplicationDate= new DateTime(2021, 05, 6)
				}
			};

			var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
			helperResultValidatorMock.Setup(x => x.ListResult(It.IsAny<Func<int, List<EmployeeVaccine>>>(), idEmployee)).Returns(employeeVaccines);

			var pageFilter = new PageFilter();

			// Act
			var result = _employeeVaccineManager.Get(idEmployee, pageFilter);

			// Assert
			for (var i = 0; i < employeeVaccines.Count; i++)
			{
				Assert.Equal(result[i].IdVaccine, employeeVaccines[i].IdVaccine);
				Assert.Equal(result[i].ApplicationDate, employeeVaccines[i].ApplicationDate);
			}
		}

		[Fact]
		public void GetMustThrowFunctionalExceptionNotFound()
		{
			// Arrange
			int idEmployee = 2;

			var employeeVaccines = new List<EmployeeVaccine>();

			var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
			helperResultValidatorMock.Setup(x => x.ListResult(It.IsAny<Func<int, List<EmployeeVaccine>>>(), idEmployee)).Returns(employeeVaccines);

			var pageFilter = new PageFilter();

			// Act
			try
			{
				var result = _employeeVaccineManager.Get(idEmployee, pageFilter);
			}
			catch (Exception ex)
			{
				// Assert
				Assert.Equal(typeof(FunctionalException), ex.GetType());
				Assert.Equal(ErrorType.NotFound, ((FunctionalException)ex).FunctionalError);
			}
		}

		[Fact]
		public void Update()
		{
			// Arrange
			int idEmployee = 1;
			var employeeVaccines = new EmployeeVaccines()
			{
				IdEmployee = idEmployee,
				Vaccines = new List<EmployeeVaccine>()
				 {
					  new EmployeeVaccine()
					  {
						  IdVaccine = 8,
						  ApplicationDate = DateTime.Now
					  },
					  new EmployeeVaccine()
					  {
						  IdVaccine = 9,
						  ApplicationDate = DateTime.Now
					  }
				 }
			};

			int affectedRows = 2;
			bool throwException = true;

			var employeeVaccinesValidatorMock = _autoMoqer.GetMock<ICustomValidator<EmployeeVaccines>>();
			employeeVaccinesValidatorMock
				.Setup(x => x.IsValid(It.IsAny<EmployeeVaccines>(), ActionType.Update))
				.Returns(true);

			var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
			helperResultValidatorMock.Setup(x => x.IntegerResult(It.IsAny<Func<EmployeeVaccines, int>>(), employeeVaccines, throwException)).Returns(affectedRows);

			_employeeVaccineManager.Update(employeeVaccines);
		}
	}
}
