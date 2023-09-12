using System.Collections.Generic;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Interfaces.Managers;

namespace VL.Health.API.Managers
{
    public class AccidentNotificationDischargeChannelManager : IAccidentNotificationDischargeChannelManager
    {
        private readonly IAccidentNotificationDischargeChannelRepository _accidentNotificationDischargeChannelRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public AccidentNotificationDischargeChannelManager(IAccidentNotificationDischargeChannelRepository accidentNotificationDischargeChannelRepository, IHelperResultValidator repositoryResultValidator)
        {
            _accidentNotificationDischargeChannelRepository = accidentNotificationDischargeChannelRepository;
            _helperResultValidator = repositoryResultValidator;
        }

        public List<AccidentNotificationDischargeChannel> Get()
        {
            var accidentNotificationDischargeChannel = _helperResultValidator.ListResult<AccidentNotificationDischargeChannel>(_accidentNotificationDischargeChannelRepository.Get);

            return accidentNotificationDischargeChannel;
        }
    }
}
