using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Validators
{
	public class EmployeePathologiesValidator : AbstractBaseValidator<EmployeePathologies>
	{
		private readonly IPathologyRepository _pathologyRepository;

		public EmployeePathologiesValidator(IPathologyRepository pathologyRepository)
		{
			_pathologyRepository = pathologyRepository;

			base.AddValidations(this.Validations);
		}

		#region Private Method

		private void Validations()
		{
			RuleFor(x => x.IdEmployee)
				.NotEmpty()
				.WithErrorCode("IdEmployeeIsRequired")
				.WithMessage("IdEmployeeIsRequired")
				.When(x => _actionType == ActionType.Update);

			RuleFor(x => x.Pathologies)
				.Must(x => !HasDuplicateItems(x, y => new { y.Id }))
				.WithErrorCode("DuplicateItemsFound")
				.WithMessage("DuplicateItemsFound")
				.When(x => _actionType == ActionType.Update);

			#region Database validations

			RuleFor(x => x.Pathologies)
				.Must(x => PathologiesExist(x))
				.WithErrorCode("AtLeastOneIdPathologyNotFound")
				.WithMessage("AtLeastOneIdPathologyNotFound")
				.When(x => _actionType == ActionType.Update);

			#endregion
		}

		private bool PathologiesExist(List<EmployeePathology> employeePathologies)
		{
			int[] ids = employeePathologies.Select(x => x.Id).Distinct().ToArray();

			return _pathologyRepository.ExistsAll(ids);
		}

		#endregion
	}
}
