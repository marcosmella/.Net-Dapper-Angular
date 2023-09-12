using System.Collections.Generic;

namespace VL.Health.Service.DTO.MedicalControl.Response
{
    public class MedicalControlResponse: MedicalControlTrackingResponse
    { 
        public List<MedicalControlTrackingResponse> Tracking { get; set; }

    }
}

