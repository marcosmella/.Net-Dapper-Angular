using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers.Interfaces;

namespace VL.Health.API.Managers
{
    public class MedicalControlActionManager : IMedicalControlActionManager
    {
        private readonly IMedicalControlActionRepository _medicalControlActionRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public MedicalControlActionManager(IMedicalControlActionRepository medicalControlActionRepository, IHelperResultValidator repositoryResultValidator)
        {
            _medicalControlActionRepository = medicalControlActionRepository;
            _helperResultValidator = repositoryResultValidator;
        }

        public List<MedicalControlAction> Get()
        {
            return _helperResultValidator.ListResult<MedicalControlAction>(_medicalControlActionRepository.Get);
        }
        public List<MedicalControlAction> GetByControlType(int idControlType)
        {
            return _helperResultValidator.ListResult<MedicalControlAction>(_medicalControlActionRepository.GetByControlType, idControlType);
        }
    }
}
