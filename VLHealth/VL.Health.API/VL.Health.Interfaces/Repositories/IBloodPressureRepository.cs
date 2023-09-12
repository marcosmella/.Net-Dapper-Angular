using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Repositories
{
	public interface IBloodPressureRepository
	{
		List<BloodPressure> Get();
		bool Exists(int id);
	}
}
