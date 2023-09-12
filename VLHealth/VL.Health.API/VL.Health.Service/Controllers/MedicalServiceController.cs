using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Audit.Client.Interface;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.MedicalService.Request;
using VL.Health.Service.DTO.MedicalService.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/medical-services")]
    [ApiController]
    public class MedicalServiceController : Controller
    {
        private readonly IMedicalServiceManager _medicalServiceManager;
        private readonly IMapper _mapper;
        private readonly IAuditClient _auditClient;

        public MedicalServiceController(IMedicalServiceManager medicalServiceManager,
                                 IMapper mapper,
                                 IAuditClient audit)
        {
            _medicalServiceManager = medicalServiceManager;
            _mapper = mapper;
            _auditClient = audit;
        }

        [HttpGet()]
        [Authorize(IdResource = 17002)]
        public ActionResult<List<MedicalServiceResponse>> Get()
        {
            var medicalServices = _medicalServiceManager.Get();

            return Ok(_mapper.Map<List<MedicalServiceResponse>>(medicalServices));
        }

        [HttpGet("{id}")]
        [Authorize(IdResource = 17025)]
        public ActionResult<MedicalServiceResponse> Get(int id)
        {
            var medicalServices = _medicalServiceManager.Get(id);

            return Ok(_mapper.Map<MedicalServiceResponse>(medicalServices));
        }

        [HttpPost]
        [Authorize(IdResource = 17017)]
        public ActionResult<int> Post(MedicalServiceRequest request)
        {
            var doctor = _mapper.Map<MedicalService>(request);
            doctor.Id = _medicalServiceManager.Create(doctor);
            _auditClient.Save(doctor, doctor.Id);

            return Ok(doctor.Id);
        }

        [HttpPut]
        [Authorize(IdResource = 17018)]
        public ActionResult Put(MedicalServiceRequest request)
        {
            var medicalService = _mapper.Map<MedicalService>(request);
            _medicalServiceManager.Update(medicalService);
            _auditClient.Save(medicalService, medicalService.Id);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(IdResource = 17019)]
        public ActionResult Delete(int id)
        {
            var medicalService = _medicalServiceManager.Delete(id);
            _auditClient.Save(medicalService, id);
            return Ok();
        }

    }
}
