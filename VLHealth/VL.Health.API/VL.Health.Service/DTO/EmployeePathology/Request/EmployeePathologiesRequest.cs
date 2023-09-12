using System.Collections.Generic;
using VL.Health.Service.DTO.EmployeeVaccine.Request;

namespace VL.Health.Service.DTO.EmployeePathology.Request
{
	public class EmployeePathologiesRequest
	{
		public int IdEmployee { get; set; }
		public List<EmployeePathologyRequest> Pathologies { get; set; }
	}
}