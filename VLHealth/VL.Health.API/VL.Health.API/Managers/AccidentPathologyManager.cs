using System.Collections.Generic;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Interfaces.Managers;

namespace VL.Health.API.Managers
{
    public class AccidentPathologyManager   : IAccidentPathologyManager
    {
        private readonly IAccidentPathologyRepository _accidentPathologyRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public AccidentPathologyManager(IAccidentPathologyRepository accidentPathologyRepository, IHelperResultValidator repositoryResultValidator)
        {
            _accidentPathologyRepository = accidentPathologyRepository;
            _helperResultValidator = repositoryResultValidator;
        }

        public List<AccidentPathology> Get()
        {
            var accidentPathology = _helperResultValidator.ListResult<AccidentPathology>(_accidentPathologyRepository.Get);

            return accidentPathology;
        }
    }
}
