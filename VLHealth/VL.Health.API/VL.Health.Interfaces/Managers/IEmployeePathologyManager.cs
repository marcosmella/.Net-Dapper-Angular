using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Managers
{
    public interface IEmployeePathologyManager

    {
        PagedList<EmployeePathology> Get(int id, PageFilter pageFilter);

        void Update(EmployeePathologies employeePathologies);
    }
}
