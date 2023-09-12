using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.AccidentStatus.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/accidents")]
    [ApiController]
    public class AccidentStatusController : Controller
    {
        private readonly IAccidentStatusManager _accidentStatusManager;
        private readonly IMapper _mapper;

        public AccidentStatusController(IAccidentStatusManager accidentStatusManager, IMapper mapper)
        {
            _accidentStatusManager = accidentStatusManager;
            _mapper = mapper;
        }

        [HttpGet("statuses")]
        [Authorize(IdResource = 17011)]
        public ActionResult<List<AccidentStatusResponse>> Get()
        {
            var statuses = _accidentStatusManager.Get();

            return Ok(_mapper.Map<List<AccidentStatusResponse>>(statuses));
        }
    }

}
