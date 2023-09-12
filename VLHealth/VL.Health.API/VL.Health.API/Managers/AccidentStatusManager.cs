using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers.Interfaces;

namespace VL.Health.API.Managers
{
    public class AccidentStatusManager   : IAccidentStatusManager
    {
        private readonly IAccidentStatusRepository _accidentStatusRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public AccidentStatusManager(IAccidentStatusRepository accidentStatusRepository, IHelperResultValidator repositoryResultValidator)
        {
            _accidentStatusRepository = accidentStatusRepository;
            _helperResultValidator = repositoryResultValidator;
        }

        public List<AccidentStatus> Get()
        {
            var accidentStatus = _helperResultValidator.ListResult<AccidentStatus>(_accidentStatusRepository.Get);

            return accidentStatus;
        }
    }
}
