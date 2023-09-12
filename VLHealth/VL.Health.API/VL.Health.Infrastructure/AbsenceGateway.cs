using Microsoft.Extensions.Configuration;
using System;
using VL.Health.Infrastructure.DTO.Absence;
using VL.Health.Infrastructure.Interfaces;
using VL.Libraries.Client.Gateway;

namespace VL.Health.Infrastructure
{
    public class AbsenceGateway : IAbsenceGateway
    {
        private readonly IServiceGateway _serviceGateway;
        private readonly Uri _url;

        public AbsenceGateway(IServiceGateway serviceGateway, IConfiguration configuration)
        {
            _serviceGateway = serviceGateway;
            _url = new Uri(configuration["AbsenceBackendUrl"]);
        }

        public AbsenceResponse GetAbsence(int idAbsence)
        {
            string resource = $@"{_url}api/absences/{idAbsence}";
            var absence = _serviceGateway.Get<AbsenceResponse>(resource).Result;
            return absence;
        }

        public AbsenceTypeResponse GetAbsenceType(int idAbsenceType)
        {
            string resource = $@"{_url}api/absences-type/{idAbsenceType}";
            var absenceType = _serviceGateway.Get<AbsenceTypeResponse>(resource).Result;
            return absenceType;
        }
        
    }
}

