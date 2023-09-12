using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.AccidentType.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
	[Route("api/accidents")]
	[ApiController]
	public class AccidentTypeController : Controller
	{
        private readonly IAccidentTypeManager _accidentTypeManager;
        private readonly IMapper _mapper;

        public AccidentTypeController(IAccidentTypeManager accidentTypeManager, IMapper mapper)
        {
            _accidentTypeManager = accidentTypeManager;
            _mapper = mapper;
        }

        [HttpGet("types")]
        [Authorize(IdResource = 17009)]
        public ActionResult<List<AccidentTypeResponse>> Get()
        {
            var accidentTypes = _accidentTypeManager.Get();

            var accidentTypeResponse = _mapper.Map<List<AccidentTypeResponse>>(accidentTypes);

            return Ok(accidentTypeResponse);
        }
    }
}
