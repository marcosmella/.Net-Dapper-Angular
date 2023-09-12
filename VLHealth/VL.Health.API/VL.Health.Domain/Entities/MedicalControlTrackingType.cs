using VL.Health.Domain.Enums;

namespace VL.Health.Domain.Entities
{
	public class MedicalControlTrackingType
	{
		public int Id { get; set; }
		public string Description { get; set; }
		public bool CreateAbsence { get; set; }
	}
}