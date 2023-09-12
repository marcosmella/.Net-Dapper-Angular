using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.BloodType.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/blood-types")]
    [ApiController]
    public class BloodTypeController : Controller
    {
        private readonly IBloodTypeManager _bloodTypeManager;
        private readonly IMapper _mapper;

        public BloodTypeController(IBloodTypeManager bloodTypeManager, IMapper mapper)
        {
            _bloodTypeManager = bloodTypeManager;
            _mapper = mapper;
        }

        [HttpGet()]
        [Authorize(IdResource = 17020)]
        public ActionResult<List<BloodTypeResponse>> Get()
        {
            var bloodTypes = _bloodTypeManager.Get();

            return Ok(_mapper.Map<List<BloodTypeResponse>>(bloodTypes));
        }
    }
}