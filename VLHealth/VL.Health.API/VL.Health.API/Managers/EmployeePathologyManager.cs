using System.Linq;
using VL.Health.API.Exceptions;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Managers;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Managers
{
    public class EmployeePathologyManager : IEmployeePathologyManager
    {
        private readonly IEmployeePathologyRepository _employeePathologyRepository;
        private readonly IHelperResultValidator _helperResultValidator;
        private readonly ICustomValidator<EmployeePathologies> _customValidator;

        public EmployeePathologyManager
            (
                IEmployeePathologyRepository employeePathologyRepository,
                IHelperResultValidator helperResultValidator,
                ICustomValidator<EmployeePathologies> customValidator
            )
        {
            _employeePathologyRepository = employeePathologyRepository;
            _helperResultValidator = helperResultValidator;
            _customValidator = customValidator;
        }

        public PagedList<EmployeePathology> Get(int id, PageFilter pageFilter)
        {
            var employeePathologies = _helperResultValidator.ListResult<EmployeePathology>(_employeePathologyRepository.Get, id);

            return PagedList<EmployeePathology>.ToPagedList(employeePathologies.AsQueryable(), pageFilter.PageNumber, pageFilter.PageSize);
        }

        public void Update(EmployeePathologies employeePathologies)
        {
            if (!_customValidator.IsValid(employeePathologies, ActionType.Update))
			{
                throw new FunctionalException(ErrorType.ValidationError, _customValidator.Errors);
            }

            _helperResultValidator.IntegerResult(_employeePathologyRepository.Update, employeePathologies, employeePathologies.Pathologies.Count > 0);
        }
    }
}
