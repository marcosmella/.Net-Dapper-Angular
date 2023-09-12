using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.AccidentComplainant.Response;
using VL.Security.Libraries.Filters;



namespace VL.Health.Service.Controllers
{
    [Route("api/accidents")]
    [ApiController]
    public class AccidentComplainantController : Controller
    {
        private readonly IAccidentComplainantManager _complainantManager;
        private readonly IMapper _mapper;

        public AccidentComplainantController(IAccidentComplainantManager complainantManager,
                                   IMapper mapper)
        {
            _complainantManager = complainantManager;
            _mapper = mapper;
        }

        [HttpGet("complainants")]
        [Authorize(IdResource = 17007)]
        public ActionResult<List<AccidentComplainantResponse>> Get()
        {
            var complainants = _complainantManager.Get();

            return Ok(_mapper.Map<List<AccidentComplainantResponse>>(complainants));
        }


    }
}
