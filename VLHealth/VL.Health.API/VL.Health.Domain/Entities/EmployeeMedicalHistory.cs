namespace VL.Health.Domain.Entities
{
	public class EmployeeMedicalHistory
	{
		public int Id { get; set; }
		public int IdPerson { get; set; }
		public int IdBloodType { get; set; }
		public int IdBloodPressure { get; set; }
		public bool IsRiskGroup { get; set; }
	}
}