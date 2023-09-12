namespace VL.Health.Service.DTO.EmployeeMedicalHistory.Response
{
	public class EmployeeMedicalHistoryResponse
	{
		public int Id { get; set; }
		public int IdPerson { get; set; }
		public int IdBloodType { get; set; }
		public int IdBloodPressure { get; set; }
		public bool IsRiskGroup { get; set; }
	}
}