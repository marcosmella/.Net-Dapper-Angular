using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Validators
{
    public class EmployeeMedicalExamValidator : AbstractValidator<EmployeeMedicalExam>, ICustomValidator<EmployeeMedicalExam>
    {
        private readonly IEmployeeMedicalExamRepository _employeeMedicalExamRepository;
        private ActionType _actionType;

        public List<string> Errors { get; private set; }

        public EmployeeMedicalExamValidator(IEmployeeMedicalExamRepository employeeMedicalExamRepository)
        {
            _employeeMedicalExamRepository = employeeMedicalExamRepository;
        }

        public bool IsValid(EmployeeMedicalExam employeeMedicalExam, ActionType actionType)
        {
            _actionType = actionType;

            Validations();

            var validationResult = base.Validate(employeeMedicalExam);

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

            RuleFor(x => x.IdEmployee)
                .NotEmpty()
                .WithErrorCode("IdEmployeeRequired")
                .WithMessage("IdEmployeeRequired")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x.IdFileType)
                .NotEmpty()
                .WithErrorCode("IdFileTypeRequired")
                .WithMessage("IdFileTypeRequired")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x.ExamDate)
               .NotEmpty()
               .WithErrorCode("ExamDateIsRequired")
               .WithMessage("ExamDateIsRequired")
               .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x)
               .Must(x => DateValidation(x.ExamDate.Value, x.ExpirationDate)).When(x => x.ExpirationDate != null)
               .WithErrorCode("ExpirationDateMustBeGreaterThanExamDate")
               .WithMessage("ExpirationDateMustBeGreaterThanExamDate")
               .When(x => _actionType == ActionType.Update || _actionType == ActionType.Create);

            #region DataBase Validator    

            RuleFor(x => x)
                .Must(x => !_employeeMedicalExamRepository.Exists(x))
                .WithMessage("EmployeeMedicalExamAlreadyExists")
                .WithErrorCode("EmployeeMedicalExamAlreadyExists")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x)
                .Must(x => _employeeMedicalExamRepository.ExistsFileType(x.IdFileType))
                .WithMessage("IdFileTypeNotExists")
                .WithErrorCode("IdFileTypeNotExists")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            #endregion

        }

        private bool DateValidation(DateTime startDate, DateTime? endDate)
        {
            return (endDate.HasValue) ? startDate < endDate : false;
            
        }
    }
}
