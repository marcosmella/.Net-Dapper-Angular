using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Repositories
{
    public interface IMedicalControlActionRepository
    {
        List<MedicalControlAction> Get();
        List<MedicalControlAction> GetByControlType(int IdControlType);
        bool Exists(int id);
    }
}
