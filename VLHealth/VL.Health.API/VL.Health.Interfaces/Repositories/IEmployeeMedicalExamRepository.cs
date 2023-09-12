using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Repositories
{
    public interface IEmployeeMedicalExamRepository
    {
        List<EmployeeMedicalExam> Get();
        EmployeeMedicalExam Get(int IdEmployeeMedicalExam);
        int Create(EmployeeMedicalExam employeeMedicalExam);
        int Update(EmployeeMedicalExam employeeMedicalExam);
        EmployeeMedicalExam Delete(int Id);
        bool Exists(EmployeeMedicalExam employeeMedicalExam);
        bool ExistsFileType(int idFileType);
    }
}
