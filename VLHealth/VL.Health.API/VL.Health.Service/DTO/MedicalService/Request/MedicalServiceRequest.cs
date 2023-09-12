using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VL.Health.Service.DTO.MedicalService.Request
{
    public class MedicalServiceRequest
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
    }
}
