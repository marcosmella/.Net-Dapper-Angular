using System.Collections.Generic;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Repositories;
using VL.Health.Interfaces.Managers;

namespace VL.Health.API.Managers
{
	public class AccidentTypeManager : IAccidentTypeManager
	{
		private readonly IAccidentTypeRepository _accidentTypeRepository;
		private readonly IHelperResultValidator _helperResultValidator;

		public AccidentTypeManager(IAccidentTypeRepository accidentTypeRepository, IHelperResultValidator helperResultValidator)
		{
			_accidentTypeRepository = accidentTypeRepository;
			_helperResultValidator = helperResultValidator;
		}

		public List<AccidentType> Get()
		{
			return _helperResultValidator.ListResult<AccidentType>(_accidentTypeRepository.Get);
		}
	}
}