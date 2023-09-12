using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Managers
{
    public interface IMedicalControlActionManager
    {
        List<MedicalControlAction> Get();
        List<MedicalControlAction> GetByControlType(int IdControlType);
    }
}
