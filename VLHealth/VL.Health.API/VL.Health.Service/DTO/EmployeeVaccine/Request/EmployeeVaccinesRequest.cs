using System.Collections.Generic;

namespace VL.Health.Service.DTO.EmployeeVaccine.Request
{
	public class EmployeeVaccinesRequest
	{
		public int IdEmployee { get; set; }
		public List<EmployeeVaccineRequest> Vaccines { get; set; }
	}
}