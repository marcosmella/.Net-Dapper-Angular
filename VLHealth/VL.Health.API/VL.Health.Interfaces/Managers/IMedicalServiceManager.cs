using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Managers
{
    public interface IMedicalServiceManager
    {
        List<MedicalService> Get();
        MedicalService Get(int id);
        int Create(MedicalService medicalService);
        void Update(MedicalService medicalService);
        MedicalService Delete(int Id);
    }
}
