using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Managers;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Managers
{
    public class EmployeeMedicalExamManager : IEmployeeMedicalExamManager
    {
        private readonly IEmployeeMedicalExamRepository _employeeMedicalExamRepository;
        private readonly IHelperResultValidator _helperResultValidator;
        private readonly ICustomValidator<EmployeeMedicalExam> _employeeMedicalExamValidator;

        public EmployeeMedicalExamManager(IEmployeeMedicalExamRepository employeeMedicalExamRepository, 
            IHelperResultValidator helperResultValidator, 
            ICustomValidator<EmployeeMedicalExam> employeeMedicalExamValidator
            )
        {
            _employeeMedicalExamRepository = employeeMedicalExamRepository;
            _helperResultValidator = helperResultValidator;
            _employeeMedicalExamValidator = employeeMedicalExamValidator;
        }

        public List<EmployeeMedicalExam> Get()
        {
            return _helperResultValidator.ListResult<EmployeeMedicalExam>(_employeeMedicalExamRepository.Get);
        }

        public EmployeeMedicalExam GetEmployeeMedicalExam(int IdEmployeeMedicalExam)
        {
            return _helperResultValidator.ObjectResult<EmployeeMedicalExam>(_employeeMedicalExamRepository.Get, IdEmployeeMedicalExam);
        }

        public int Create(EmployeeMedicalExam employeeMedicalExam)
        {
            if (!_employeeMedicalExamValidator.IsValid(employeeMedicalExam, ActionType.Create))
            {
                throw new FunctionalException(ErrorType.ValidationError, _employeeMedicalExamValidator.Errors);
            }

            return _helperResultValidator.IntegerResult(_employeeMedicalExamRepository.Create, employeeMedicalExam);
        }

        public void Update(EmployeeMedicalExam employeeMedicalExam)
        {
            if (!_employeeMedicalExamValidator.IsValid(employeeMedicalExam, ActionType.Update))
            {
                throw new FunctionalException(ErrorType.ValidationError, _employeeMedicalExamValidator.Errors);
            }
            _helperResultValidator.IntegerResult(_employeeMedicalExamRepository.Update, employeeMedicalExam);
        }

        public EmployeeMedicalExam Delete(int id)
        {
            return _helperResultValidator.ObjectResult<EmployeeMedicalExam>(_employeeMedicalExamRepository.Delete, id);
            
        }
    }
}
