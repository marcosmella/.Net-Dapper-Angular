using System;
using System.Collections.Generic;

namespace VL.Health.Domain.Entities
{
    public class MedicalControl : MedicalControlTracking
    {
        public List<MedicalControlTracking> Tracking { get; set; }
    }
}
