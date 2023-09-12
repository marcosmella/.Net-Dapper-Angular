using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Repositories
{
	public interface IEmployeeMedicalHistoryRepository
	{
		EmployeeMedicalHistory Get(int idEmployee);
		bool Exists(int id);
		int Create(EmployeeMedicalHistory medicalHistory);
		int Update(EmployeeMedicalHistory medicalHistory);
	}
}