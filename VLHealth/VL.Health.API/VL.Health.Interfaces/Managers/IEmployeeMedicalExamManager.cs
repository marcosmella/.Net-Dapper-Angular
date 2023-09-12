using System.Collections.Generic;
using VL.Health.Domain.Entities;

namespace VL.Health.Interfaces.Managers
{
    public interface IEmployeeMedicalExamManager
    {
        List<EmployeeMedicalExam> Get();
        EmployeeMedicalExam GetEmployeeMedicalExam(int IdEmployeeMedicalExam);
        int Create(EmployeeMedicalExam employeeMedicalExam);
        void Update(EmployeeMedicalExam employeeMedicalExam);
        EmployeeMedicalExam Delete(int Id);
    }
}
