using System.Collections.Generic;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Managers
{
    public class VaccineManager : IVaccineManager
    {
        private readonly IVaccineRepository _vaccineRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public VaccineManager(IVaccineRepository vaccineRepository, IHelperResultValidator helperResultValidator)
        {
            _vaccineRepository = vaccineRepository;
            _helperResultValidator = helperResultValidator;
        }

        public List<Vaccine> Get()
        {
            return _helperResultValidator.ListResult<Vaccine>(_vaccineRepository.Get);
        }
    }
}