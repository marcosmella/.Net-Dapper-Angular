using System.Collections.Generic;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Managers
{
    public class AccidentReopeningManager : IAccidentReopeningManager
    {
        private readonly IAccidentReopeningRepository _accidentReopeningRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public AccidentReopeningManager(IAccidentReopeningRepository accidentReopeningRepository, IHelperResultValidator helperResultValidator)
        {
            _accidentReopeningRepository = accidentReopeningRepository;
            _helperResultValidator = helperResultValidator;
        }

        public List<AccidentReopening> Get()
        {
            return _helperResultValidator.ListResult<AccidentReopening>(_accidentReopeningRepository.Get);
        }
    }
}
