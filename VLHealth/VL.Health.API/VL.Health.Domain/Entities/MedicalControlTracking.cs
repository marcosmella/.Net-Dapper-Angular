using System;
using System.Collections.Generic;

namespace VL.Health.Domain.Entities
{
    public class MedicalControlTracking
    {
        public int Id { get; set; }
        public Employee Employee { get; set; }
        public DateTime Date { get; set; }
        public MedicalControlType ControlType { get; set; }
        public MedicalControlAction Action { get; set; }
        public MedicalService MedicalService { get; set; }
        public Doctor OccupationalDoctor { get; set; }
        public string PrivateDoctorName { get; set; }
        public string PrivateDoctorEnrollment { get; set; }
        public string Diagnosis { get; set; }
        public Absence Absence { get; set; }
        public int? IdFile { get; set; }
        public int BreakTime { get; set; }
		public DateTime? TestDate {get; set;}
		public Boolean? TestResult {get; set;}
        public int? IdParent { get; set; }
        public MedicalControlTrackingType TrackingType { get; set; }
        public List<Pathology> Pathologies { get; set; }
        public int? IdFileComplaint { get; set; }
    }
}
