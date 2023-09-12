using System;
using System.Collections.Generic;

namespace VL.Health.Service.DTO.MedicalControlTracking.Request
{
    public class MedicalControlRequest
    {
        public int Id { get; set; }
        public int IdEmployee { get; set; }
        public DateTime Date { get; set; }
        public int IdControlType { get; set; }
        public int? IdAction { get; set; }
        public int IdMedicalService { get; set; }
        public int? IdOccupationalDoctor { get; set; }
        public string PrivateDoctorName { get; set; }
        public string PrivateDoctorEnrollment { get; set; }
        public string Diagnosis { get; set; }
        public int? IdAbsence { get; set; }
        public int? IdFile { get; set; }
        public int? BreakTime { get; set; }
        public DateTime? TestDate { get; set; }
        public Boolean? TestResult { get; set; }
        public int? IdParent { get; set; }
        public int? IdTrackingType { get; set; }
        public int[] Pathologies { get; set; }
    }
}