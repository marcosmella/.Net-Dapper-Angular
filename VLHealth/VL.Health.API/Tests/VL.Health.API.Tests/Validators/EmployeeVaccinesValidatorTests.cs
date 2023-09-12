using Xunit;
using AutoMoqCore;
using System;
using System.Collections.Generic;
using VL.Health.API.Validators;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;
using Moq;

namespace VL.Health.API.Tests.Managers
{
	public class EmployeeVaccinesValidatorTests
	{
		private readonly AutoMoqer _autoMoqer;
		private readonly ICustomValidator<EmployeeVaccines> _employeeVaccinesValidator;

		public EmployeeVaccinesValidatorTests()
		{
			_autoMoqer = new AutoMoqer();
			_employeeVaccinesValidator = _autoMoqer.Resolve<EmployeeVaccinesValidator>();
		}

		[Fact]
		public void IsValid_Suceeds_Should_Not_Find_Duplicates()
		{
			// Arrange
			var employeeVaccines = new EmployeeVaccines()
			{
				IdEmployee = 1,
				Vaccines = new List<EmployeeVaccine>
				{
					new EmployeeVaccine
					{
						IdVaccine = 1,
						ApplicationDate = new DateTime(2021, 5, 6)
					},
					new EmployeeVaccine
					{
						IdVaccine = 2,
						ApplicationDate = new DateTime(2021, 5, 6)
					},
					new EmployeeVaccine
					{
						IdVaccine = 1,
						ApplicationDate = new DateTime(2019, 1, 12)
					},
					new EmployeeVaccine
					{
						IdVaccine = 1,
						ApplicationDate = new DateTime(2019, 1, 13)
					}
				}
			};

			var vaccineRepositoryMock =_autoMoqer.GetMock<IVaccineRepository>();
			vaccineRepositoryMock.Setup(x => x.ExistsAll(It.IsAny<int[]>())).Returns(true);

			// Act
			var result = _employeeVaccinesValidator.IsValid(employeeVaccines, ActionType.Update);

			// Assert
			Assert.True(result);
			Assert.Empty(_employeeVaccinesValidator.Errors);
		}

		[Fact]
		public void IsValid_Fails_Should_Find_Missing_IdEmployee()
		{
			// Arrange
			var employeeVaccines = new EmployeeVaccines()
			{
				Vaccines = new List<EmployeeVaccine>
				{
					new EmployeeVaccine
					{
						IdVaccine = 1,
						ApplicationDate = new DateTime(2021, 5, 6)
					},
					new EmployeeVaccine
					{
						IdVaccine = 1,
						ApplicationDate = new DateTime(2019, 5, 7)
					}
				}
			};

			var vaccineRepositoryMock = _autoMoqer.GetMock<IVaccineRepository>();
			vaccineRepositoryMock.Setup(x => x.ExistsAll(It.IsAny<int[]>())).Returns(true);

			// Act
			var result = _employeeVaccinesValidator.IsValid(employeeVaccines, ActionType.Update);

			// Assert
			Assert.False(result);
			Assert.Equal("IdEmployeeIsRequired", _employeeVaccinesValidator.Errors[0]);
		}

		[Fact]
		public void IsValid_Fails_Should_Find_Two_Duplicates()
		{
			// Arrange
			var employeeVaccines = new EmployeeVaccines()
			{
				IdEmployee = 1,
				Vaccines = new List<EmployeeVaccine>
				{
					new EmployeeVaccine
					{
						IdVaccine = 1,
						ApplicationDate = new DateTime(2021, 5, 6)
					},
					new EmployeeVaccine
					{
						IdVaccine = 1,
						ApplicationDate = new DateTime(2019, 1, 12)
					},
					new EmployeeVaccine
					{
						IdVaccine = 1,
						ApplicationDate = new DateTime(2021, 5, 6)
					}
				}
			};

			var vaccineRepositoryMock = _autoMoqer.GetMock<IVaccineRepository>();
			vaccineRepositoryMock.Setup(x => x.ExistsAll(It.IsAny<int[]>())).Returns(true);

			// Act
			var result = _employeeVaccinesValidator.IsValid(employeeVaccines, ActionType.Update);

			// Assert
			Assert.False(result);
			Assert.Equal("DuplicateItemsFound", _employeeVaccinesValidator.Errors[0]);
		}

		[Fact]
		public void IsValid_Fails_Should_Not_Find_IdVaccines_In_Repository()
		{
			// Arrange
			var employeeVaccines = new EmployeeVaccines()
			{
				IdEmployee = 1,
				Vaccines = new List<EmployeeVaccine>
				{
					new EmployeeVaccine
					{
						IdVaccine = 1,
						ApplicationDate = new DateTime(2021, 5, 6)
					},
					new EmployeeVaccine
					{
						IdVaccine = 2,
						ApplicationDate = new DateTime(2019, 1, 12)
					}
				}
			};

			int[] existingIds = new int[] { 1, 3 };
			var vaccineRepositoryMock = _autoMoqer.GetMock<IVaccineRepository>();
			vaccineRepositoryMock.Setup(x => x.ExistsAll(existingIds)).Returns(true);

			// Act
			var result = _employeeVaccinesValidator.IsValid(employeeVaccines, ActionType.Update);

			// Assert
			Assert.False(result);
			Assert.Equal("AtLeastOneIdVaccineNotFound", _employeeVaccinesValidator.Errors[0]);
		}
	}
}
