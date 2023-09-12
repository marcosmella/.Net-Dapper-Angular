using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VL.Audit.Client.Interface;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.MedicalControlTracking.Request;
using VL.Health.Service.DTO.MedicalControl.Response;
using VL.Security.Libraries.Filters;
using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;

namespace VL.Health.Service.Controllers
{
    [Route("api/medical-controls")]
    [ApiController]

    public class MedicalControlController : Controller
    {
        private readonly IMedicalControlManager _medicalControlManager;
        private readonly IMapper _mapper;
        private readonly IAuditClient _auditClient;

        public MedicalControlController(IMedicalControlManager controlManager,
                                    IMapper mapper,
                                    IAuditClient audit)
        {
            _medicalControlManager = controlManager;
            _mapper = mapper;
            _auditClient = audit;
        }

        [HttpGet("{id}")]
        [Authorize(IdResource = 17041)]
        public ActionResult<MedicalControlResponse> Get(int id, bool tracking = false)
        {
            
            var medicalControl = _medicalControlManager.Get(id, tracking);

            return Ok(_mapper.Map<MedicalControlResponse>(medicalControl));
        }

        [HttpPost]
        [Authorize(IdResource = 17044)]
        public ActionResult<int> Post(MedicalControlRequest request)
        {
            var medicalControl = _mapper.Map<MedicalControlTracking>(request);
            medicalControl.Id = _medicalControlManager.Create(medicalControl);
            _auditClient.Save(medicalControl, medicalControl.Id);

            return Ok(medicalControl.Id);
        }

        [HttpPut]
        [Authorize(IdResource = 17045)]
        public ActionResult Put(MedicalControlRequest request)
        {
            var medicalControl = _mapper.Map<MedicalControlTracking>(request);
            _medicalControlManager.Update(medicalControl);
            _auditClient.Save(medicalControl, medicalControl.Id);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(IdResource = 17043)]
        public async Task<ActionResult> Delete(int id)
        {
            var medicalControl = await _medicalControlManager.Delete(id);
            await _auditClient.Save(medicalControl, id);
            return Ok();
        }

        [HttpPut("absence-rectify")]
        [Authorize(IdResource = 17049)]
        public ActionResult RectifyAbsence(RectifyAbsenceRequest request)
        {
            var medicalControl = _medicalControlManager.RectifyAbsence(request.OldAbsenceId, request.NewAbsenceId);
            _auditClient.Save(medicalControl, medicalControl.Id);
            return Ok();
        }

        [HttpGet("absences/{id}")]
        [Authorize(IdResource = 17050)]
        public ActionResult<MedicalControlResponse> GetByAbsenceId(int id)
        {
            var medicalControl = _medicalControlManager.GetByAbsenceId(id);

            return Ok(_mapper.Map<MedicalControlResponse>(medicalControl));
        }


        [HttpPatch("{id}")]
        [Authorize(IdResource = 17051)]
        public ActionResult Patch(int id, JsonPatchDocument operation)
        {
            _medicalControlManager.Patch(id, operation);
            _auditClient.Save(operation.Operations, id);
            return Ok();
        }


    }
}
