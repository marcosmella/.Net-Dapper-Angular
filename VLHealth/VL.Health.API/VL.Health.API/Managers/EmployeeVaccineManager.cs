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
    public class EmployeeVaccineManager : IEmployeeVaccineManager
    {
        private readonly IEmployeeVaccineRepository _employeeVaccineRepository;
        private readonly IHelperResultValidator _helperResultValidator;
        private readonly ICustomValidator<EmployeeVaccines> _customValidator;

        public EmployeeVaccineManager
            (
                IEmployeeVaccineRepository employeeVaccineRepository,
                IHelperResultValidator helperResultValidator,
                ICustomValidator<EmployeeVaccines> customValidator
            )
        {
            _employeeVaccineRepository = employeeVaccineRepository;
            _helperResultValidator = helperResultValidator;
            _customValidator = customValidator;
        }

        public PagedList<EmployeeVaccine> Get(int id, PageFilter pageFilter)
        {
            var employeeVaccines = _helperResultValidator.ListResult<EmployeeVaccine>(_employeeVaccineRepository.Get, id);

            return PagedList<EmployeeVaccine>.ToPagedList(employeeVaccines.AsQueryable(), pageFilter.PageNumber, pageFilter.PageSize);
        }

		public void Update(EmployeeVaccines employeeVaccines)
		{
            if (!_customValidator.IsValid(employeeVaccines, ActionType.Update))
            {
                throw new FunctionalException(ErrorType.ValidationError, _customValidator.Errors);
            }

            _helperResultValidator.IntegerResult(_employeeVaccineRepository.Update, employeeVaccines, employeeVaccines.Vaccines.Count > 0);
		}
	}
}
