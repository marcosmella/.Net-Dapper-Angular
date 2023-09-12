using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using VL.Audit.Client.Interface;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.EmployeeVaccine.Request;
using VL.Health.Service.DTO.EmployeeVaccine.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/clinical-records")]
    [ApiController]
    public class EmployeeVaccineController : Controller
    {
        private readonly IEmployeeVaccineManager _employeeVaccineManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditClient _auditClient;

        public EmployeeVaccineController
            (
                IEmployeeVaccineManager employeeVaccineManager,
                IMapper mapper,
                IHttpContextAccessor httpContextAccessor,
                IAuditClient auditClient
            )
        {
            _employeeVaccineManager = employeeVaccineManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _auditClient = auditClient;
        }

        [HttpGet("employees/{id}/vaccines")]
        [Authorize(IdResource = 17027)]
        public ActionResult<PagedList<EmployeeVaccineResponse>> Get(int id, [FromQuery] PageFilter pageFilter)
        {
            var employeeVaccines = _employeeVaccineManager.Get(id, pageFilter);

            var metadata = PaginationDataFrom(employeeVaccines);
            _httpContextAccessor.HttpContext.Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var response = _mapper.Map<List<EmployeeVaccine>, List<EmployeeVaccineResponse>>(employeeVaccines);

            return Ok(response);
        }

		[HttpPut("employees/vaccines")]
		[Authorize(IdResource = 17033)]
		public ActionResult Put(EmployeeVaccinesRequest employeeVaccinesRequest)
		{
			var employeeVaccines = _mapper.Map<EmployeeVaccinesRequest, EmployeeVaccines>(employeeVaccinesRequest);

			_employeeVaccineManager.Update(employeeVaccines);

			_auditClient.Save(employeeVaccines, null, employeeVaccinesRequest.IdEmployee, (int)EntityType.Employee);

			return Ok();
		}

		#region Private Methods

		private object PaginationDataFrom(PagedList<EmployeeVaccine> items)
        {
            var metadata = new
            {
                items.TotalCount,
                items.PageSize,
                items.CurrentPage,
                items.TotalPages,
                items.HasNext,
                items.HasPrevious
            };

            return metadata;
        }

        #endregion
    }
}
