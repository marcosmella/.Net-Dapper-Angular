using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Repositories
{
    public interface IDoctorRepository
    {
        List<Doctor> Get();
        Doctor Get(int IdDoctor);
        int Create(Doctor doctor);
        int Update(Doctor doctor);
        Doctor Delete(int Id);
        bool Exists(Doctor doctor);
        bool Exists(int id);

    }
}
