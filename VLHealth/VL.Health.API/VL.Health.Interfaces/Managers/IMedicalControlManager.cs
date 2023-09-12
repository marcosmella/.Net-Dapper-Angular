using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;
using VL.Health.Domain.Entities;


namespace VL.Health.Interfaces.Managers
{
    public interface IMedicalControlManager
    {
        MedicalControl Get(int id, bool tracking);
        int Create(MedicalControlTracking medicalControl);
        void Update(MedicalControlTracking medicalControl);
        Task<MedicalControlTracking> Delete(int id);
        MedicalControlTracking RectifyAbsence(int oldIdAbsence, int newIdAbsence);
        MedicalControlTracking GetByAbsenceId(int idAbsence);
        void Patch(int id, JsonPatchDocument operation);
    }
}
