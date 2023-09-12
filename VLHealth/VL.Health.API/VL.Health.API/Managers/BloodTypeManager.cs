using System.Collections.Generic;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Managers
{
	public class BloodTypeManager : IBloodTypeManager
	{
        private readonly IBloodTypeRepository _bloodTypeRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public BloodTypeManager(IBloodTypeRepository bloodTypeRepository, IHelperResultValidator helperResultValidator)
        {
            _bloodTypeRepository = bloodTypeRepository;
            _helperResultValidator = helperResultValidator;
        }

        public List<BloodType> Get()
        {
            return _helperResultValidator.ListResult<BloodType>(_bloodTypeRepository.Get);
        }
	}
}