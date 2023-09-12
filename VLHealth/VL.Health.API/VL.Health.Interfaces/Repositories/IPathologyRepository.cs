using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Repositories
{
    public interface IPathologyRepository
    {
        List<Pathology> Get(string filter);
        Pathology Get(int id);
        bool ExistsAll(int[] ids);
    }
}
