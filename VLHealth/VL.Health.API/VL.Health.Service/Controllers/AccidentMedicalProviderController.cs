using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.AccidentMedicalProvider.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/accidents")]
    [ApiController]
    public class AccidentMedicalProviderController : Controller
    {
        private readonly IAccidentMedicalProviderManager _accidentMedicalProviderManager;
        private readonly IMapper _mapper;

        public AccidentMedicalProviderController(IAccidentMedicalProviderManager accidentMedicalProviderManager, IMapper mapper)
        {
            _accidentMedicalProviderManager = accidentMedicalProviderManager;
            _mapper = mapper;
        }

        [HttpGet("medical-providers")]
        [Authorize(IdResource = 17005)]
        public ActionResult<List<AccidentMedicalProviderResponse>> Get()
        {
            var medicalProviders = _accidentMedicalProviderManager.Get();

            return Ok(_mapper.Map<List<AccidentMedicalProviderResponse>>(medicalProviders));
        }
    }
}
