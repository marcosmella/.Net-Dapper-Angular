using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers.Interfaces;

namespace VL.Health.API.Managers
{
    public class PathologyManager : IPathologyManager
    {
        private readonly IPathologyRepository _pathologyRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public PathologyManager(IPathologyRepository pathologyRepository, IHelperResultValidator helperResultValidator)
        {
            _pathologyRepository = pathologyRepository;
            _helperResultValidator = helperResultValidator;
        }

        public List<Pathology> Get(string filter)
        {
            var pathologies = _pathologyRepository.Get(filter);

            if (pathologies.Count == 0)
            {                
                throw new FunctionalException(ErrorType.NotFound);
            }

            return pathologies;
        }

        public Pathology GetById(int id)
        {
            return _helperResultValidator.ObjectResult<Pathology>(_pathologyRepository.Get, id);
        }
    }
}
