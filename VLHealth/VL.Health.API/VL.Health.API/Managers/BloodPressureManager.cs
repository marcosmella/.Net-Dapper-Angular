using System;
using System.Collections.Generic;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Managers
{
	public class BloodPressureManager : IBloodPressureManager
	{
        private readonly IBloodPressureRepository _bloodPressureRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public BloodPressureManager(IBloodPressureRepository bloodPressureRepository, IHelperResultValidator helperResultValidator)
        {
            _bloodPressureRepository = bloodPressureRepository;
            _helperResultValidator = helperResultValidator;
        }

        public List<BloodPressure> Get()
        {
            return _helperResultValidator.ListResult<BloodPressure>(_bloodPressureRepository.Get);
        }
    }
}
