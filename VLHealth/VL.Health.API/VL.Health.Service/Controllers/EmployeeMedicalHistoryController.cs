using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VL.Audit.Client.Interface;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.EmployeeMedicalHistory.Request;
using VL.Health.Service.DTO.EmployeeMedicalHistory.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/clinical-records")]
    [ApiController]
    public class EmployeeMedicalHistoryController : Controller
    {
        private readonly IEmployeeMedicalHistoryManager _medicalHistoryManager;
        private readonly IMapper _mapper;
        private readonly IAuditClient _auditClient;

        public EmployeeMedicalHistoryController
            (
                IEmployeeMedicalHistoryManager medicalHistoryManager,
                IAuditClient auditClient,
                IMapper mapper
            )
        {
            _medicalHistoryManager = medicalHistoryManager;
            _mapper = mapper;
            _auditClient = auditClient;
        }

        [HttpGet("employees/{id}/medical-history")]
        [Authorize(IdResource = 17022)]
        public ActionResult<EmployeeMedicalHistory> Get(int id)
        {
            var medicalHistory = _medicalHistoryManager.Get(id);

            var response = _mapper.Map<EmployeeMedicalHistory, EmployeeMedicalHistoryResponse>(medicalHistory);

            return Ok(response);
        }

        [HttpPost("employees/medical-history")]
        [Authorize(IdResource = 17023)]
        public ActionResult<int> Create(EmployeeMedicalHistoryRequest medicalHistoryRequest)
        {
            var medicalHistory = _mapper.Map<EmployeeMedicalHistoryRequest, EmployeeMedicalHistory>(medicalHistoryRequest);

            medicalHistory.Id = _medicalHistoryManager.Create(medicalHistory);

            _auditClient.Save<EmployeeMedicalHistory>(medicalHistory, medicalHistory.Id, medicalHistory.IdPerson, (int)EntityType.Employee);

            return Ok(medicalHistory.Id);
        }

        [HttpPut("employees/medical-history")]
        [Authorize(IdResource = 17024)]
        public ActionResult Update(EmployeeMedicalHistoryRequest medicalHistoryRequest)
        {
            var medicalHistory = _mapper.Map<EmployeeMedicalHistoryRequest, EmployeeMedicalHistory>(medicalHistoryRequest);

            _medicalHistoryManager.Update(medicalHistory);

            _auditClient.Save<EmployeeMedicalHistory>(medicalHistory, medicalHistory.Id, medicalHistory.IdPerson, (int)EntityType.Employee);

            return Ok();
        }
    }
}