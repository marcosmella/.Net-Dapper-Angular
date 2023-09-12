namespace VL.Health.Service.DTO.EmployeeMedicalHistory.Request
{
	public class EmployeeMedicalHistoryRequest
	{
		public int Id { get; set; }
		public int IdPerson { get; set; }
		public int IdBloodType { get; set; }
		public int IdBloodPressure { get; set; }
		public bool IsRiskGroup { get; set; }
	}
}