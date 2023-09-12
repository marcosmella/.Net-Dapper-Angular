using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Service.DTO.Pathology.Response;
using VL.Security.Libraries.Filters;

namespace VL.Health.Service.Controllers
{
    [Route("api/pathologies")]
    [ApiController]
    public class PathologyController : Controller
    {
        private readonly IPathologyManager _pathologyManager;
        private readonly IMapper _mapper;

        public PathologyController(IPathologyManager pathologyManager,
                                   IMapper mapper)
        {
            _pathologyManager = pathologyManager;
            _mapper = mapper;
        }

        [HttpGet()]
        [Authorize(IdResource = 17003)]
        public ActionResult<List<PathologyResponse>> Get([FromQuery] string pathologyFilter)
        {
            var pathologies = _pathologyManager.Get(pathologyFilter);

            return Ok(_mapper.Map<List<PathologyResponse>>(pathologies));
        }

        [HttpGet("{id}")]
        [Authorize(IdResource = 17048)]
        public ActionResult<PathologyResponse> GetById(int id)
        {
            var pathologies = _pathologyManager.GetById(id);

            return Ok(_mapper.Map<PathologyResponse>(pathologies));
        }
    }
}