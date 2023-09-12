using System;

namespace VL.Health.Infrastructure.DTO.Absence
{
    public class AbsenceResponse
    {
        public int Id { get; set; }
        public int IdAbsenceType { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Description { get; set; }
        public int? IdPathology { get; set; }
    }
}
