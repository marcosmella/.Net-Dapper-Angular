using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Domain.Entities;

namespace VL.Health.API.Managers
{
    public class AccidentComplaintChannelManager : IAccidentComplaintChannelManager
    {
        private readonly IAccidentComplaintChannelRepository _accidentComplaintChannelRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public AccidentComplaintChannelManager(IAccidentComplaintChannelRepository accidentComplaintChannelRepository, IHelperResultValidator repositoryResultValidator)
        {
            _accidentComplaintChannelRepository = accidentComplaintChannelRepository;
            _helperResultValidator = repositoryResultValidator;
        }

        public List<AccidentComplaintChannel> Get()
        {
            var accidentComplaintChannels = _helperResultValidator.ListResult<AccidentComplaintChannel>(_accidentComplaintChannelRepository.Get);

            return accidentComplaintChannels;
        }
    }
}
