using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using VL.Audit.Client.Interface;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.EmployeePathology.Request;
using VL.Health.Service.DTO.EmployeePathology.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/clinical-records")]
    [ApiController]
    public class EmployeePathologyController : Controller
    {
        private readonly IEmployeePathologyManager _employeePathologyManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditClient _auditClient;

        public EmployeePathologyController
            (
                IEmployeePathologyManager employeePathologyManager,
                IMapper mapper,
                IHttpContextAccessor httpContextAccessor,
                IAuditClient auditClient
            )
        {
            _employeePathologyManager = employeePathologyManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _auditClient = auditClient;
        }

        [HttpGet("employees/{id}/pathologies")]
        [Authorize(IdResource = 17035)]
        public ActionResult<PagedList<EmployeePathologyResponse>> Get(int id, [FromQuery] PageFilter pageFilter)
        {
            var employeePathologies = _employeePathologyManager.Get(id, pageFilter);

            var metadata = PaginationDataFrom(employeePathologies);
            _httpContextAccessor.HttpContext.Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var response = _mapper.Map<List<EmployeePathology>, List<EmployeePathologyResponse>>(employeePathologies);

            return Ok(response);
        }

        [HttpPut("employees/pathologies")]
        [Authorize(IdResource = 17034)]
        public ActionResult Put(EmployeePathologiesRequest employeePathologiesRequest)
        {
            var employeePathologies = _mapper.Map<EmployeePathologiesRequest, EmployeePathologies>(employeePathologiesRequest);

            _employeePathologyManager.Update(employeePathologies);

            _auditClient.Save(employeePathologies, null, employeePathologiesRequest.IdEmployee, (int)EntityType.Employee);

            return Ok();
        }

        #region Private Methods

        private object PaginationDataFrom(PagedList<EmployeePathology> items)
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
