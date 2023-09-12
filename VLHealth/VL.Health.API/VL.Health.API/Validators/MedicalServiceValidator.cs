using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Validators
{
    public class MedicalServiceValidator : AbstractValidator<MedicalService>, ICustomValidator<MedicalService>
    {

        private readonly IMedicalServiceRepository _medicalServiceRepository;
        private ActionType _actionType;

        public List<string> Errors { get; private set; }

        public MedicalServiceValidator(IMedicalServiceRepository medicalServiceRepository)
        {
            _medicalServiceRepository = medicalServiceRepository;
        }

        public bool IsValid(MedicalService medicalService, ActionType actionType)
        {
            _actionType = actionType;

            Validations();

            var validationResult = base.Validate(medicalService);

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

            RuleFor(x => x.Company)
                .NotEmpty()
                .WithErrorCode("CompanyRequired")
                .WithMessage("CompanyIsRequired")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithErrorCode("PhoneRequired")
                .WithMessage("PhoneIsRequired")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            #region DataBase Validator    

            RuleFor(x => x)
                .Must(x => !_medicalServiceRepository.NameExists(x))
                .WithMessage("MedicalServiceAlreadyExists")
                .WithErrorCode("MedicalServiceAlreadyExists")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            #endregion


        }

    }
}
