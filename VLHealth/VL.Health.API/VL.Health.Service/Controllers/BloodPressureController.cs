using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.BloodPressure.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/blood-pressures")]
    [ApiController]
    public class BloodPressureController : Controller
    {
        private readonly IBloodPressureManager _bloodPressureManager;
        private readonly IMapper _mapper;

        public BloodPressureController(IBloodPressureManager bloodPressureManager, IMapper mapper)
        {
            _bloodPressureManager = bloodPressureManager;
            _mapper = mapper;
        }

        [HttpGet()]
        [Authorize(IdResource = 17021)]
        public ActionResult<List<BloodPressureResponse>> Get()
        {
            var bloodPressures = _bloodPressureManager.Get();

            return Ok(_mapper.Map<List<BloodPressureResponse>>(bloodPressures));
        }
    }
}