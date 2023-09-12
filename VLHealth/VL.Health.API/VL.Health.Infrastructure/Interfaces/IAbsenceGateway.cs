
using VL.Health.Infrastructure.DTO.Absence;

namespace VL.Health.Infrastructure.Interfaces
{
    public interface IAbsenceGateway
    {
        AbsenceResponse GetAbsence(int idAbsence);
        AbsenceTypeResponse GetAbsenceType(int idAbsenceType);
    }
}
