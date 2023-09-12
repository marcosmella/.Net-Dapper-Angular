using System;

namespace VL.Health.Domain.Entities
{
    public class EmployeeMedicalExam
    {
        public int Id { get; set; }
        public int IdEmployee { get; set; }
        public int IdFileType { get; set; }
        public int IdFile { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ExamDate { get; set; }
    }
}
