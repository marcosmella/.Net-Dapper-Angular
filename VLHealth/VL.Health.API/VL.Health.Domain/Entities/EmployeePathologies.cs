using System.Collections.Generic;

namespace VL.Health.Domain.Entities
{
	public class EmployeePathologies
	{
		public int IdEmployee { get; set; }
		public List<EmployeePathology> Pathologies { get; set; }
	}
}
