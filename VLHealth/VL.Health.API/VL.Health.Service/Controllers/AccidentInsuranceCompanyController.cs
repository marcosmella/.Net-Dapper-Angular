using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.AccidentInsuranceCompany.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/accidents")]
    [ApiController]
    public class AccidentInsuranceCompanyController : Controller
    {
        private readonly IAccidentInsuranceCompanyManager _accidentInsuranceCompanyManager;
        private readonly IMapper _mapper;

        public AccidentInsuranceCompanyController(IAccidentInsuranceCompanyManager accidentInsuranceCompanyManager, IMapper mapper)
        {
            _accidentInsuranceCompanyManager = accidentInsuranceCompanyManager;
            _mapper = mapper;
        }

        [HttpGet("insurance-companies")]
        [Authorize(IdResource = 17010)]
        public ActionResult<List<AccidentInsuranceCompanyResponse>> Get()
        {
            var insuranceCompanies = _accidentInsuranceCompanyManager.Get();

            return Ok(_mapper.Map<List<AccidentInsuranceCompanyResponse>>(insuranceCompanies));
        }
    }

}
