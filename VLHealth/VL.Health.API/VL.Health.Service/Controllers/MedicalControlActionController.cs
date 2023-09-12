using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.MedicalControlAction.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/medical-controls")]
    [ApiController]

    public class MedicalControlActionController : Controller
    {

        private readonly IMedicalControlActionManager _medicalControlActionManager;
        private readonly IMapper _mapper;

        public MedicalControlActionController(IMedicalControlActionManager medicalControlActionManager,
                                    IMapper mapper)
        {
            _medicalControlActionManager = medicalControlActionManager;
            _mapper = mapper;
        }

        [HttpGet("actions")]
        [Authorize(IdResource = 17016)]
        public ActionResult<List<MedicalControlActionResponse>> Get()
        {
            var medicalControlActions = _medicalControlActionManager.Get();

            return Ok(_mapper.Map<List<MedicalControlActionResponse>>(medicalControlActions));
        }

        [HttpGet("control-types/{id}/actions")] 
        [Authorize(IdResource = 17032)]
        public ActionResult<List<MedicalControlActionResponse>> GetByControlType(int id)
        {
            var medicalControlActions = _medicalControlActionManager.GetByControlType(id);

            return Ok(_mapper.Map<List<MedicalControlActionResponse>>(medicalControlActions));
        }



    }
}
