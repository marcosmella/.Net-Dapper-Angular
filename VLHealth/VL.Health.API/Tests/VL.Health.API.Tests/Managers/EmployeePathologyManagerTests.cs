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
	public class EmployeePathologyManagerTests
	{
		private readonly AutoMoqer _autoMoqer;
		private readonly IEmployeePathologyManager _employeePathologyManager;

		public EmployeePathologyManagerTests()
		{
			_autoMoqer = new AutoMoqer();
			_employeePathologyManager = _autoMoqer.Resolve<EmployeePathologyManager>();
		}

		[Fact]
		public void Get()
		{
			// Arrange
			int idEmployee = 1;
			var employeePathologies = new List<EmployeePathology>
			{
				new EmployeePathology
				{
					Id = 1
				}
			};

			var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
			helperResultValidatorMock.Setup(x => x.ListResult(It.IsAny<Func<int, List<EmployeePathology>>>(), idEmployee)).Returns(employeePathologies);

			var pageFilter = new PageFilter();

			// Act
			var result = _employeePathologyManager.Get(idEmployee, pageFilter);

			// Assert
			for (var i = 0; i < employeePathologies.Count; i++)
			{
				Assert.Equal(result[i].Id, employeePathologies[i].Id);
			}
		}

		[Fact]
		public void GetMustThrowFunctionalExceptionNotFound()
		{
			// Arrange
			int idEmployee = 2;

			var employeePathologies = new List<EmployeePathology>();

			var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
			helperResultValidatorMock.Setup(x => x.ListResult(It.IsAny<Func<int, List<EmployeePathology>>>(), idEmployee)).Returns(employeePathologies);

			var pageFilter = new PageFilter();

			// Act
			try
			{
				var result = _employeePathologyManager.Get(idEmployee, pageFilter);
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
			var employeePathologies = new EmployeePathologies()
			{
				IdEmployee = idEmployee,
				Pathologies = new List<EmployeePathology>()
				 {
					  new EmployeePathology()
					  {
						  Id = 2
					  },
					  new EmployeePathology()
					  {
						  Id = 3,
					  }
				 }
			};

			int affectedRows = 2;

			bool throwException = true;


			var employeePathologiesValidatorMock = _autoMoqer.GetMock<ICustomValidator<EmployeePathologies>>();
			employeePathologiesValidatorMock
				.Setup(x => x.IsValid(It.IsAny<EmployeePathologies>(), ActionType.Update))
				.Returns(true);

			var helperResultValidatorMock = _autoMoqer.GetMock<IHelperResultValidator>();
			helperResultValidatorMock.Setup(x => x.IntegerResult(It.IsAny<Func<EmployeePathologies, int>>(), employeePathologies, throwException)).Returns(affectedRows);

			_employeePathologyManager.Update(employeePathologies);
		}
	}
}
