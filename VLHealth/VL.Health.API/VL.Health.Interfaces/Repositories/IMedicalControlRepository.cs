using System;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Repositories
{
    public interface IMedicalControlRepository
    {
        MedicalControl Get(int Id);
        MedicalControl GetWithTracking(int Id);
        MedicalControlTracking Delete(int Id);
        int Create(MedicalControlTracking medicalControl);
        int Update(MedicalControlTracking medicalControl);
        bool AbsenceRelationshipExists(int idCurrentMedicalControl, int idAbsence);
        bool IsValidDate(DateTime date, int idAbsence);
        bool Exists(int id);
        bool TrackingDateValid(MedicalControlTracking medicalControl);
        MedicalControlTracking GetLastControlTracking(int id);
        bool IsParentOrLastControlTracking(int id);
        MedicalControlTracking GetByAbsenceId(int idAbsence);
        int UpdateAbsenceId(MedicalControlTracking medicalControl);
    }
}
