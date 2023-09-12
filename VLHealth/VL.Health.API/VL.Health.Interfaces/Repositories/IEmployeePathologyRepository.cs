using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Repositories
{
    public interface IEmployeePathologyRepository
    {
        List<EmployeePathology> Get(int id);
        int Update(EmployeePathologies employeePathologies);
    }
}
