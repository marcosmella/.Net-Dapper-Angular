using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Repositories
{
	public interface IBloodTypeRepository
	{
		List<BloodType> Get();
		bool Exists(int id);
	}
}
