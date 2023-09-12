using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Repositories
{
    public interface IVaccineRepository
    {
		List<Vaccine> Get();
		bool Exists(int id);
		bool ExistsAll(int[] ids);
	}
}