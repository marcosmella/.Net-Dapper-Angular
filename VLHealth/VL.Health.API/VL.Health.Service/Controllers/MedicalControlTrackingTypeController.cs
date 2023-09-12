using AutoMapper;
using VL.Security.Libraries.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.BloodType.Response;

namespace VL.Health.Service.Controllers
{
    [Route("api/medical-controls/tracking-types")]
    [ApiController]
    public class MedicalControlTrackingTypeController : Controller
    {
        private readonly IMedicalControlTrackingTypeManager _trackingTypeManager;
        private readonly IMapper _mapper;

        public MedicalControlTrackingTypeController(IMedicalControlTrackingTypeManager trackingTypeManager, IMapper mapper)
        {
            _trackingTypeManager = trackingTypeManager;
            _mapper = mapper;
        }

        [HttpGet()]
        [Authorize(IdResource = 17047)]
        public ActionResult<List<MedicalControlTrackingTypeResponse>> Get()
        {
            var trackingTypes = _trackingTypeManager.Get();

            return Ok(_mapper.Map<List<MedicalControlTrackingTypeResponse>>(trackingTypes));
        }
    }
}