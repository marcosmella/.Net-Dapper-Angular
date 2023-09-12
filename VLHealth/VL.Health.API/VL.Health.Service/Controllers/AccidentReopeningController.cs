using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.AccidentReopening.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/accidents")]
    [ApiController]
    public class AccidentReopeningController : Controller
    {
        private readonly IAccidentReopeningManager _accidentReopeningManager;
        private readonly IMapper _mapper;

        public AccidentReopeningController(IAccidentReopeningManager accidentReopeningManager, IMapper mapper)
        {
            _accidentReopeningManager = accidentReopeningManager;
            _mapper = mapper;
        }

        [HttpGet("reopenings")]
        [Authorize(IdResource = 17012)]
        public ActionResult<List<AccidentReopeningResponse>> Get()
        {
            var accidentReopenings = _accidentReopeningManager.Get();

            return Ok(_mapper.Map<List<AccidentReopeningResponse>>(accidentReopenings));
        }

    }
}
