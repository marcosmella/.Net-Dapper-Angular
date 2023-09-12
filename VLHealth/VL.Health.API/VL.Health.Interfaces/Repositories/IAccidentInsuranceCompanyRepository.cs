using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Repositories
{
    public interface IAccidentInsuranceCompanyRepository
    {
        List<AccidentInsuranceCompany> Get();
    }
}
