using System;
using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Repositories
{
    public interface IMedicalControlTypeRepository
    {
        List<MedicalControlType> Get();
        bool Exists(int id);
    }
}
