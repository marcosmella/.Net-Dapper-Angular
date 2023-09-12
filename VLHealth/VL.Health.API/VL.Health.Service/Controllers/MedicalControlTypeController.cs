using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.MedicalControlType.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/medical-control-types")]
    [ApiController]
    public class MedicalControlTypeController : Controller
    {
        private readonly IMedicalControlTypeManager _medicalControlTypeManager;
        private readonly IMapper _mapper;

        public MedicalControlTypeController(IMedicalControlTypeManager medicalControlTypeManager,
                                 IMapper mapper)
        {
            _medicalControlTypeManager = medicalControlTypeManager;
            _mapper = mapper;
        }

        [HttpGet()]
        [Authorize(IdResource = 17001)]
        public ActionResult<List<MedicalControlTypeResponse>> Get()
        {
            var medicalControlTypes = _medicalControlTypeManager.Get();

            return Ok(_mapper.Map<List<MedicalControlTypeResponse>>(medicalControlTypes));
        }
    }
}
