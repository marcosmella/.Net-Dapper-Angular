using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VL.Audit.Client.Interface;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.EmployeeMedicalExam.Request;
using VL.Health.Service.DTO.EmployeeMedicalExam.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/medical-exams")]
    [ApiController]
    public class EmployeeMedicalExamController : Controller
    {
        private readonly IEmployeeMedicalExamManager _employeeMedicalExamManager;
        private readonly IMapper _mapper;
        private readonly IAuditClient _auditClient;

        public EmployeeMedicalExamController(
                                 IEmployeeMedicalExamManager employeeMedicalExamManager,
                                 IMapper mapper,
                                 IAuditClient audit)
        {
            _employeeMedicalExamManager = employeeMedicalExamManager;
            _mapper = mapper;
            _auditClient = audit;
        }

        [HttpGet()]
        [Authorize(IdResource = 17036)]
        public ActionResult<List<EmployeeMedicalExamResponse>> Get()
        {
            var employeeMedicalExams = _employeeMedicalExamManager.Get();

            return Ok(_mapper.Map<List<EmployeeMedicalExamResponse>>(employeeMedicalExams));
        }

        [HttpGet("{id}")]
        [Authorize(IdResource = 17037)]
        public ActionResult<EmployeeMedicalExamResponse> Get(int id)
        {
            var employeeMedicalExam = _employeeMedicalExamManager.GetEmployeeMedicalExam(id);

            return Ok(_mapper.Map<EmployeeMedicalExamResponse>(employeeMedicalExam));
        }

        [HttpPost]
        [Authorize(IdResource = 17038)]
        public ActionResult<int> Post(EmployeeMedicalExamRequest request)
        {
            var employeeMedicalExam = _mapper.Map<EmployeeMedicalExam>(request);
            employeeMedicalExam.Id = _employeeMedicalExamManager.Create(employeeMedicalExam);
            _auditClient.Save(employeeMedicalExam, employeeMedicalExam.Id, employeeMedicalExam.IdEmployee, (int)EntityType.Employee);

            return Ok(employeeMedicalExam.Id);
        }

        [HttpPut]
        [Authorize(IdResource = 17039)]
        public ActionResult Put(EmployeeMedicalExamRequest request)
        {
            var employeeMedicalExam = _mapper.Map<EmployeeMedicalExam>(request);
            _employeeMedicalExamManager.Update(employeeMedicalExam);
            _auditClient.Save(employeeMedicalExam, employeeMedicalExam.Id, employeeMedicalExam.IdEmployee, (int)EntityType.Employee);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(IdResource = 17040)]
        public ActionResult Delete(int id)
        {
            var employeeMedicalExam = _employeeMedicalExamManager.Delete(id);
            _auditClient.Save(employeeMedicalExam, id);
            return Ok();
        }
    }
}
