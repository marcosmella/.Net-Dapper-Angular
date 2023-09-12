using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Repositories
{
    public interface IMedicalServiceRepository
    {
        List<MedicalService> Get();
        MedicalService Get(int id);
        int Create(MedicalService medicalService);
        int Update(MedicalService medicalService);
        MedicalService Delete(int Id);
        bool NameExists(MedicalService medicalService);
        bool Exists(int id);
    }
}
