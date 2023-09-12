using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Validators
{
    public class DoctorValidator : AbstractValidator<Doctor>, ICustomValidator<Doctor>
    {

        private readonly IDoctorRepository _doctorRepository;
        private ActionType _actionType;

        public List<string> Errors { get; private set; }

        public DoctorValidator(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public bool IsValid(Doctor doctor, ActionType actionType)
        {
            _actionType = actionType;

            Validations();

            var validationResult = base.Validate(doctor);

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

            RuleFor(x => x.DocumentNumber)
                .NotEmpty()
                .WithErrorCode("DocumentRequired")
                .WithMessage("DocumentRequired")
                .Length(1, 25)
                .WithErrorCode("InvalidDocumentLength")
                .WithMessage("InvalidDocumentLength")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x.Enrollment)
                .NotEmpty()
                .WithErrorCode("EnrollmentRequired")
                .WithMessage("EnrollmentRequired")
                .Length(1, 15)
                .WithErrorCode("InvalidEnrollmentLength")
                .WithMessage("InvalidEnrollmentLength")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            #region DataBase Validator    
            
            RuleFor(x => x)            
                .Must(x => !_doctorRepository.Exists(x))
                .WithMessage("DoctorAlreadyExists")
                .WithErrorCode("DoctorAlreadyExists")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            #endregion

        }

    }
}
