using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Managers
{
    public interface IEmployeeVaccineManager
    {
        PagedList<EmployeeVaccine> Get(int id, PageFilter pageFilter);

        void Update(EmployeeVaccines employeeVaccines);
    }
}
