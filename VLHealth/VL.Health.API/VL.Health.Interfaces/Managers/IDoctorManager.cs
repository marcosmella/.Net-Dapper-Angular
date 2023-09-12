using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Managers
{
    public interface IDoctorManager
    {
        List<Doctor> Get();
        Doctor GetDoctor(int IdDoctor);
        int Create(Doctor doctor);
        void Update(Doctor doctor);
        Doctor Delete(int Id);
    }
}