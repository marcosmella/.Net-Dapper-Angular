using System.Collections.Generic;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Interfaces.Managers;

namespace VL.Health.API.Managers
{
    public class AccidentInsuranceCompanyManager : IAccidentInsuranceCompanyManager
    {
        private readonly IAccidentInsuranceCompanyRepository _accidentInsuranceCompanyRepository;
        private readonly IHelperResultValidator _helperResultValidator;

        public AccidentInsuranceCompanyManager(IAccidentInsuranceCompanyRepository accidentInsuranceCompanyRepository, IHelperResultValidator repositoryResultValidator)
        {
            _accidentInsuranceCompanyRepository = accidentInsuranceCompanyRepository;
            _helperResultValidator = repositoryResultValidator;
        }

        public List<AccidentInsuranceCompany> Get()
        {
            var accidentInsuranceCompany = _helperResultValidator.ListResult<AccidentInsuranceCompany>(_accidentInsuranceCompanyRepository.Get);

            return accidentInsuranceCompany;
        }
    }
}
