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
	public class EmployeePathologiesValidatorTests
	{
		private readonly AutoMoqer _autoMoqer;
		private readonly ICustomValidator<EmployeePathologies> _employeePathologiesValidator;

		public EmployeePathologiesValidatorTests()
		{
			_autoMoqer = new AutoMoqer();
			_employeePathologiesValidator = _autoMoqer.Resolve<EmployeePathologiesValidator>();
		}

		[Fact]
		public void IsValid_Suceeds_Should_Not_Find_Duplicates()
		{
			// Arrange
			var employeePathologies = new EmployeePathologies()
			{
				IdEmployee = 1,
				Pathologies = new List<EmployeePathology>
				{
					new EmployeePathology { Id = 1 },                   
					new EmployeePathology { Id = 2 },
					new EmployeePathology { Id = 3 },
					new EmployeePathology { Id = 4 }
				}
			};

			var pathologyRepositoryMock = _autoMoqer.GetMock<IPathologyRepository>();
			pathologyRepositoryMock.Setup(x => x.ExistsAll(It.IsAny<int[]>())).Returns(true);

			// Act
			var result = _employeePathologiesValidator.IsValid(employeePathologies, ActionType.Update);

			// Assert
			Assert.True(result);
			Assert.Empty(_employeePathologiesValidator.Errors);
		}

		[Fact]
		public void IsValid_Fails_Should_Find_Missing_IdEmployee()
		{
			// Arrange
			var employeePathologies = new EmployeePathologies()
			{
				Pathologies = new List<EmployeePathology>
				{
					new EmployeePathology { Id = 1 },
					new EmployeePathology { Id = 2 },
					new EmployeePathology { Id = 3 },
					new EmployeePathology { Id = 4 }
				}
			};

			var pathologyRepositoryMock = _autoMoqer.GetMock<IPathologyRepository>();
			pathologyRepositoryMock.Setup(x => x.ExistsAll(It.IsAny<int[]>())).Returns(true);

			// Act
			var result = _employeePathologiesValidator.IsValid(employeePathologies, ActionType.Update);

			// Assert
			Assert.False(result);
			Assert.Equal("IdEmployeeIsRequired", _employeePathologiesValidator.Errors[0]);
		}

		[Fact]
		public void IsValid_Fails_Should_Find_Two_Duplicates()
		{
			// Arrange
			var employeePathologies = new EmployeePathologies()
			{
				IdEmployee = 1,
				Pathologies = new List<EmployeePathology>
				{
					new EmployeePathology { Id = 1 },
					new EmployeePathology { Id = 2 },
					new EmployeePathology { Id = 3 },
					new EmployeePathology { Id = 2 }
				}
			};

			var pathologyRepositoryMock = _autoMoqer.GetMock<IPathologyRepository>();
			pathologyRepositoryMock.Setup(x => x.ExistsAll(It.IsAny<int[]>())).Returns(true);

			// Act
			var result = _employeePathologiesValidator.IsValid(employeePathologies, ActionType.Update);

			// Assert
			Assert.False(result);
			Assert.Equal("DuplicateItemsFound", _employeePathologiesValidator.Errors[0]);
		}

		[Fact]
		public void IsValid_Fails_Should_Not_Find_IdVaccines_In_Repository()
		{
			// Arrange
			var employeePathologies = new EmployeePathologies()
			{
				IdEmployee = 1,
				Pathologies = new List<EmployeePathology>
				{
					new EmployeePathology { Id = 1 },
					new EmployeePathology { Id = 2 },
					new EmployeePathology { Id = 3 },
					new EmployeePathology { Id = 4 }
				}
			};

			int[] existingIds = new int[] { 1, 3 };
			var pathologyRepositoryMock = _autoMoqer.GetMock<IPathologyRepository>();
			pathologyRepositoryMock.Setup(x => x.ExistsAll(existingIds)).Returns(true);

			// Act
			var result = _employeePathologiesValidator.IsValid(employeePathologies, ActionType.Update);

			// Assert
			Assert.False(result);
			Assert.Equal("AtLeastOneIdPathologyNotFound", _employeePathologiesValidator.Errors[0]);
		}
	}
}

