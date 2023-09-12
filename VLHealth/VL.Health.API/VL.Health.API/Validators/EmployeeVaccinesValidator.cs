using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Validators
{
	public class EmployeeVaccinesValidator : AbstractBaseValidator<EmployeeVaccines>
	{
		private readonly IVaccineRepository _vaccineRepository;
		
		public EmployeeVaccinesValidator(IVaccineRepository vaccineRepository)
		{
			_vaccineRepository = vaccineRepository;

			base.AddValidations(this.Validations);
		}

		#region Private Methods

		private void Validations()
		{
			RuleFor(x => x.IdEmployee)
				.NotEmpty()
				.WithErrorCode("IdEmployeeIsRequired")
				.WithMessage("IdEmployeeIsRequired")
				.When(x => base._actionType == ActionType.Update);

			RuleFor(x => x.Vaccines)
				.Must(x => !base.HasDuplicateItems(x, y => new { y.IdVaccine, y.ApplicationDate }))
				.WithErrorCode("DuplicateItemsFound")
				.WithMessage("DuplicateItemsFound")
				.When(x => base._actionType == ActionType.Update);

			#region Database validations

			RuleFor(x => x.Vaccines)
				.Must(x => VaccinesExist(x))
				.WithErrorCode("AtLeastOneIdVaccineNotFound")
				.WithMessage("AtLeastOneIdVaccineNotFound")
				.When(x => base._actionType == ActionType.Update);

			#endregion
		}

		private bool VaccinesExist(List<EmployeeVaccine> employeeVaccines)
		{
			int[] ids = employeeVaccines.Select(x => x.IdVaccine).Distinct().ToArray();

			return _vaccineRepository.ExistsAll(ids);
		}

		#endregion
	}
}
