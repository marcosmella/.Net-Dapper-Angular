using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Managers
{
    public class MedicalControlTypeManager : IMedicalControlTypeManager
    {
        private readonly IMedicalControlTypeRepository _medicalControlTypeRepository;

        public MedicalControlTypeManager(IMedicalControlTypeRepository medicalControlTypeRepository)
        {
            _medicalControlTypeRepository = medicalControlTypeRepository;
        }

        public List<MedicalControlType> Get()
        {
            var medicalControlTypes = _medicalControlTypeRepository.Get();

            if (medicalControlTypes.Count == 0)
            {
                throw new FunctionalException(ErrorType.NotFound);
            }

            return medicalControlTypes;
        }
    }
}
