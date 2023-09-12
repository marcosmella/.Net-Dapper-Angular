using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.Vaccine.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/vaccines")]
    [ApiController]
    public class VaccineController : Controller
    {
        private readonly IVaccineManager _vaccineManager;
        private readonly IMapper _mapper;

        public VaccineController(IVaccineManager vaccineManager, IMapper mapper)
        {
            _vaccineManager = vaccineManager;
            _mapper = mapper;
        }

        [HttpGet()]
        [Authorize(IdResource = 17031)]
        public ActionResult<List<VaccineResponse>> Get()
        {
            var vaccines = _vaccineManager.Get();

            return Ok(_mapper.Map<List<VaccineResponse>>(vaccines));
        }
    }
}
