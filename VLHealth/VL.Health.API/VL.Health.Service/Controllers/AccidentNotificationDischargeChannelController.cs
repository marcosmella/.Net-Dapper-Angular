using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.AccidentNotificationDischargeChannel.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/accidents")]
    [ApiController]
    public class AccidentNotificationDischargeChannelController : Controller
    {
        private readonly IAccidentNotificationDischargeChannelManager _accidentMedicalNotificationDischargeChannelManager;
        private readonly IMapper _mapper;

        public AccidentNotificationDischargeChannelController(IAccidentNotificationDischargeChannelManager accidentMedicalNotificationDischargeChannelManager, IMapper mapper)
        {
            _accidentMedicalNotificationDischargeChannelManager = accidentMedicalNotificationDischargeChannelManager;
            _mapper = mapper;
        }

        [HttpGet("notification-discharge-channels")]
        [Authorize(IdResource = 17006)]
        public ActionResult<List<AccidentNotificationDischargeChannelResponse>> Get()
        {
            var notificationDischargeChannel = _accidentMedicalNotificationDischargeChannelManager.Get();

            return Ok(_mapper.Map<List<AccidentNotificationDischargeChannelResponse>>(notificationDischargeChannel));
        }
    }
}
