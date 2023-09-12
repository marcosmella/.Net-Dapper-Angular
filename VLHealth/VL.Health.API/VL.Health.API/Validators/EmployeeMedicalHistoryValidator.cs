using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Infrastructure.Interfaces;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Validators
{
    public class EmployeeMedicalHistoryValidator : AbstractValidator<EmployeeMedicalHistory>, ICustomValidator<EmployeeMedicalHistory>
    {
        private readonly IEmployeeMedicalHistoryRepository _employeeMedicalHistoryRepository;
        private readonly IBloodTypeRepository _bloodTypeRepository;
        private readonly IBloodPressureRepository _bloodPressureRepository;
        private readonly IPersonGateway _personGateway;
        private ActionType _actionType;

        public List<string> Errors { get; private set; }

        public EmployeeMedicalHistoryValidator(
            IEmployeeMedicalHistoryRepository medicalHistoryRepository,
            IBloodTypeRepository bloodTypeRepository,
            IBloodPressureRepository bloodPressureRepository,
            IPersonGateway personGateway
            )
        {
            _employeeMedicalHistoryRepository = medicalHistoryRepository;
            _bloodTypeRepository = bloodTypeRepository;
            _bloodPressureRepository = bloodPressureRepository;
            _personGateway = personGateway;
        }

        public bool IsValid(EmployeeMedicalHistory medicalHistory, ActionType actionType)
        {
            _actionType = actionType;

            Validations();

            var validationResult = base.Validate(medicalHistory);

            Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();

            return validationResult.IsValid;
        }

        public void Validations()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithErrorCode("IdIsRequired")
                .WithMessage("IdIsRequired")
                .When(x => _actionType == ActionType.Update);

            RuleFor(x => x.IdPerson)
                .GreaterThan(0)
                .WithErrorCode("IdPersonRequired")
                .WithMessage("IdPersonRequired")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x.IdBloodType)
                .GreaterThan(0)
                .WithErrorCode("IdBloodTypeRequired")
                .WithMessage("IdBloodTypeRequired")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x.IdBloodPressure)
                .GreaterThan(0)
                .WithErrorCode("IdBloodPressureRequired")
                .WithMessage("IdBloodPressureRequired")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            #region DataBase Validator    

            RuleFor(x => x.IdPerson)
                .Must(x => !_employeeMedicalHistoryRepository.Exists(x))
                .WithMessage("MedicalHistoryAlreadyExists")
                .WithErrorCode("MedicalHistoryAlreadyExists")
                .When(x => _actionType == ActionType.Create);

            RuleFor(x => x.IdPerson)
				.Must(x => _employeeMedicalHistoryRepository.Exists(x))
				.WithMessage("MedicalHistoryDoesNotExist")
				.WithErrorCode("MedicalHistoryDoesNotExist")
				.When(x => _actionType == ActionType.Update || _actionType == ActionType.Delete);

            RuleFor(x => x.IdPerson)
                .Must(x => _personGateway.Exists(x))
                .WithMessage("PersonDoesNotExist")
                .WithErrorCode("PersonDoesNotExist")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x.IdBloodPressure)
                .Must(x => _bloodPressureRepository.Exists(x))
                .WithMessage("BloodPressureDoesNotExist")
                .WithErrorCode("BloodPressureDoesNotExist")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x.IdBloodType)
               .Must(x => _bloodTypeRepository.Exists(x))
               .WithMessage("BloodTypeDoesNotExist")
               .WithErrorCode("BloodTypeDoesNotExist")
               .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            #endregion
        }
    }
}
