using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Audit.Client.Interface;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.Doctor.Request;
using VL.Health.Service.DTO.Doctor.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/doctors")]
    [ApiController]
    public class DoctorController : Controller
    {
        private readonly IDoctorManager _doctorManager;
        private readonly IMapper _mapper;
        private readonly IAuditClient _auditClient;

        public DoctorController(IDoctorManager doctorManager,
                                 IMapper mapper,
                                 IAuditClient audit)
        {
            _doctorManager = doctorManager;
            _mapper = mapper;
            _auditClient = audit;
        }

        [HttpGet()]
        [Authorize(IdResource = 17000)]
        public ActionResult<List<DoctorResponse>> Get()
        {
            var doctors = _doctorManager.Get();

            return Ok(_mapper.Map<List<DoctorResponse>>(doctors));
        }

        [HttpGet("{id}")]
        [Authorize(IdResource = 17026)]
        public ActionResult<DoctorResponse> Get(int id )
        {
            var doctor = _doctorManager.GetDoctor(id);

            return Ok(_mapper.Map<DoctorResponse>(doctor));
        }

        [HttpPost]
        [Authorize(IdResource = 17013)]
        public ActionResult<int> Post(DoctorRequest request)
        {
            var doctor = _mapper.Map<Doctor>(request);
            doctor.Id = _doctorManager.Create(doctor);
            _auditClient.Save(doctor, doctor.Id);

            return Ok(doctor.Id);
        }

        [HttpPut]
        [Authorize(IdResource = 17014)]
        public ActionResult Put(DoctorRequest request)
        {
            var doctor = _mapper.Map<Doctor>(request);
            _doctorManager.Update(doctor);
            _auditClient.Save(doctor, doctor.Id);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(IdResource = 17015)]
        public ActionResult Delete(int id)
        {
            var doctor = _doctorManager.Delete(id);
            _auditClient.Save(doctor, id);
            return Ok();
        }

    }
}