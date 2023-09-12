using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.AccidentComplaintChannel.Response;
using VL.Security.Libraries.Filters;



namespace VL.Health.Service.Controllers
{
    [Route("api/accidents")]
    [ApiController]
    public class AccidentComplaintChannelController : Controller
    {
        private readonly IAccidentComplaintChannelManager _complaintChannelManager;
        private readonly IMapper _mapper;

        public AccidentComplaintChannelController(IAccidentComplaintChannelManager complaintChannelManager,
                                   IMapper mapper)
        {
            _complaintChannelManager = complaintChannelManager;
            _mapper = mapper;
        }

        [HttpGet("complaint-channels")]
        [Authorize(IdResource = 17008)]
        public ActionResult<List<AccidentComplaintChannelResponse>> Get()
        {
            var complaintChannels = _complaintChannelManager.Get();

            return Ok(_mapper.Map<List<AccidentComplaintChannelResponse>>(complaintChannels));
        }


    }
}
