using System.Collections.Generic;
using VL.Health.API.Exceptions;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.API.Validators.Interfaces;

namespace VL.Health.API.Managers
{
    public class MedicalServiceManager : IMedicalServiceManager
    {
        private readonly IMedicalServiceRepository _medicalServiceRepository;
        private readonly IHelperResultValidator _helperResultValidator;
        private readonly ICustomValidator<MedicalService> _medicalServiceValidator;

        public MedicalServiceManager(IMedicalServiceRepository medicalServiceRepository, IHelperResultValidator helperResultValidator, ICustomValidator<MedicalService> medicalServiceValidator)
        {
            _medicalServiceRepository = medicalServiceRepository;
            _helperResultValidator = helperResultValidator;
            _medicalServiceValidator = medicalServiceValidator;
        }

        public List<MedicalService> Get()
        {
            return _helperResultValidator.ListResult<MedicalService>(_medicalServiceRepository.Get);
        }

        public MedicalService Get(int id)
        {
            return _helperResultValidator.ObjectResult<MedicalService>(_medicalServiceRepository.Get, id);
        }

        public int Create(MedicalService medicalService)
        {
            if (!_medicalServiceValidator.IsValid(medicalService, ActionType.Create))
            {
                throw new FunctionalException(ErrorType.ValidationError, _medicalServiceValidator.Errors);
            }

            return _helperResultValidator.IntegerResult(_medicalServiceRepository.Create, medicalService);
        }

        public void Update(MedicalService medicalService)
        {
            if (!_medicalServiceValidator.IsValid(medicalService, ActionType.Update))
            {
                throw new FunctionalException(ErrorType.ValidationError, _medicalServiceValidator.Errors);
            }

            _helperResultValidator.IntegerResult(_medicalServiceRepository.Update, medicalService);
        }

        public MedicalService Delete(int id)
        {
            return _helperResultValidator.ObjectResult<MedicalService>(_medicalServiceRepository.Delete, id);
        }

    }
}
