using System.Collections.Generic;

namespace VL.Health.Domain.Entities
{
	public class EmployeeVaccines
	{
		public int IdEmployee { get; set; }
		public List<EmployeeVaccine> Vaccines { get; set; }
	}
}
