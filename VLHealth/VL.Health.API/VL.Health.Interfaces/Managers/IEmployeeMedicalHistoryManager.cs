using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Managers
{
	public interface IEmployeeMedicalHistoryManager
	{
		EmployeeMedicalHistory Get(int idPerson);
		int Create(EmployeeMedicalHistory medicalHistory);
		void Update(EmployeeMedicalHistory medicalHistory);
	}
}