using VL.Health.API.Exceptions;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Managers;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Managers
{
	public class EmployeeMedicalHistoryManager : IEmployeeMedicalHistoryManager
	{
		private readonly IEmployeeMedicalHistoryRepository _medicalHistoryRepository;
		private readonly IHelperResultValidator _helperResultValidator;
		private readonly ICustomValidator<EmployeeMedicalHistory> _medicalHistoryValidator;

		public EmployeeMedicalHistoryManager(
			IEmployeeMedicalHistoryRepository medicalHistoryRepository,
			IHelperResultValidator helperResultValidator,
			ICustomValidator<EmployeeMedicalHistory> medicalHistoryValidator)
		{
			_medicalHistoryRepository = medicalHistoryRepository;
			_helperResultValidator = helperResultValidator;
			_medicalHistoryValidator = medicalHistoryValidator;
		}

		public EmployeeMedicalHistory Get(int idPerson)
		{
			return _helperResultValidator.ObjectResult<EmployeeMedicalHistory>(_medicalHistoryRepository.Get, idPerson);
		}

		public int Create(EmployeeMedicalHistory medicalHistory)
		{
			if (!_medicalHistoryValidator.IsValid(medicalHistory, ActionType.Create))
			{
				throw new FunctionalException(ErrorType.ValidationError, _medicalHistoryValidator.Errors);
			}

			return _helperResultValidator.IntegerResult<EmployeeMedicalHistory>(_medicalHistoryRepository.Create, medicalHistory);
		}

		public void Update(EmployeeMedicalHistory medicalHistory)
		{
			if (!_medicalHistoryValidator.IsValid(medicalHistory, ActionType.Update))
			{
				throw new FunctionalException(ErrorType.ValidationError, _medicalHistoryValidator.Errors);
			}

			_helperResultValidator.IntegerResult<EmployeeMedicalHistory>(_medicalHistoryRepository.Update, medicalHistory);
		}
	}
}
