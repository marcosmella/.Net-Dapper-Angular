using System;

namespace VL.Health.Service.DTO.Doctor.Request
{
    public class DoctorRequest
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Enrollment { get; set; }
        public DateTime? EnrollmentExpirationDate { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? DocumentExpirationDate { get; set; }
    }
}
