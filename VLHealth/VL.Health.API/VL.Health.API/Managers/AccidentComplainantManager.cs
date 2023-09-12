using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Domain.Entities;

namespace VL.Health.API.Managers
{
    public class AccidentComplainantManager : IAccidentComplainantManager
    {
        private readonly IAccidentComplainantRepository _accidentComplainantRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public AccidentComplainantManager(IAccidentComplainantRepository accidentComplainantRepository, IHelperResultValidator repositoryResultValidator)
        {
            _accidentComplainantRepository = accidentComplainantRepository;
            _helperResultValidator = repositoryResultValidator;
        }

        public List<AccidentComplainant> Get()
        {
            var accidentComplainant = _helperResultValidator.ListResult<AccidentComplainant>(_accidentComplainantRepository.Get);

            return accidentComplainant;
        }
    }
}
