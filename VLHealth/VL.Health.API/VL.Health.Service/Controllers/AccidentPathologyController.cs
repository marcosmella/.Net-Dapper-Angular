using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.AccidentPathology.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/accidents")]
    [ApiController]
    public class AccidentPathologyController : Controller
    {
        private readonly IAccidentPathologyManager _accidentPathologyManager;
        private readonly IMapper _mapper;

        public AccidentPathologyController(IAccidentPathologyManager accidentPathologyManager, IMapper mapper)
        {
            _accidentPathologyManager = accidentPathologyManager;
            _mapper = mapper;
        }

        [HttpGet("pathologies")]
        [Authorize(IdResource = 17004)]
        public ActionResult<List<AccidentPathologyResponse>> Get()
        {
            var pathologies = _accidentPathologyManager.Get();

            return Ok(_mapper.Map<List<AccidentPathologyResponse>>(pathologies));
        }
    }

}
