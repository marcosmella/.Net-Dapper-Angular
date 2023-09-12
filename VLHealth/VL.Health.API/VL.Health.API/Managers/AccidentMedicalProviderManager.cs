using System.Collections.Generic;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Interfaces.Managers;

namespace VL.Health.API.Managers
{
    public class AccidentMedicalProviderManager : IAccidentMedicalProviderManager
    {
        private readonly IAccidentMedicalProviderRepository _accidentMedicalProviderRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public AccidentMedicalProviderManager(IAccidentMedicalProviderRepository accidentMedicalProvideryRepository, IHelperResultValidator repositoryResultValidator)
        {
            _accidentMedicalProviderRepository = accidentMedicalProvideryRepository;
            _helperResultValidator = repositoryResultValidator;
        }

        public List<AccidentMedicalProvider> Get()
        {
            var accidentMedicalProvider =_helperResultValidator.ListResult<AccidentMedicalProvider>(_accidentMedicalProviderRepository.Get);

            return accidentMedicalProvider;
        }
    }
}
