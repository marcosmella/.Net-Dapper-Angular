using System.Collections.Generic;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.API.Validators.Interfaces;
using VL.Health.API.Exceptions;
using VL.Health.Domain.Enums;

namespace VL.Health.API.Managers
{
    public class DoctorManager : IDoctorManager
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IHelperResultValidator _helperResultValidator;
        private readonly ICustomValidator<Doctor> _doctorValidator;

        public DoctorManager(IDoctorRepository doctorRepository, IHelperResultValidator helperResultValidator, ICustomValidator<Doctor> doctorValidator)
        {
            _doctorRepository = doctorRepository;
            _helperResultValidator = helperResultValidator;
            _doctorValidator = doctorValidator;
        }

        public List<Doctor> Get()
        {
            return _helperResultValidator.ListResult<Doctor>(_doctorRepository.Get);
        }

        public Doctor GetDoctor(int IdDoctor)
        {
            return _helperResultValidator.ObjectResult<Doctor>(_doctorRepository.Get, IdDoctor);
        }

        public int Create(Doctor doctor)
        {
            if (!_doctorValidator.IsValid(doctor, ActionType.Create))
            {
                throw new FunctionalException(ErrorType.ValidationError, _doctorValidator.Errors);
            }

            return _helperResultValidator.IntegerResult(_doctorRepository.Create, doctor);
        }

        public void Update(Doctor doctor)
        {
            if (!_doctorValidator.IsValid(doctor, ActionType.Update))
            {
                throw new FunctionalException(ErrorType.ValidationError, _doctorValidator.Errors);
            }
            _helperResultValidator.IntegerResult(_doctorRepository.Update, doctor);
        }

        public Doctor Delete(int id)
        {
            return _helperResultValidator.ObjectResult<Doctor>(_doctorRepository.Delete, id);
        }

    }
}
