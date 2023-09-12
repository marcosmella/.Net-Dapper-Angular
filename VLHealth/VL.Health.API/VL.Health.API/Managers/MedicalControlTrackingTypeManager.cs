using System.Collections.Generic;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Managers
{
	public class MedicalControlTrackingTypeManager : IMedicalControlTrackingTypeManager
	{
        private readonly IMedicalControlTrackingTypeRepository _trackingTypeRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public MedicalControlTrackingTypeManager(IMedicalControlTrackingTypeRepository trackingTypeRepository, IHelperResultValidator helperResultValidator)
        {
            _trackingTypeRepository = trackingTypeRepository;
            _helperResultValidator = helperResultValidator;
        }

        public List<MedicalControlTrackingType> Get()
        {
            return _helperResultValidator.ListResult<MedicalControlTrackingType>(_trackingTypeRepository.Get);
        }

    }
}