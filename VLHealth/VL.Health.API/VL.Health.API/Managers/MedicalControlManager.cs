using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.API.Validators.Interfaces;
using VL.Health.API.Exceptions;
using VL.Health.Domain.Enums;
using Microsoft.AspNetCore.JsonPatch;
using VL.Health.Infrastructure.DTO.WebApiFile;
using VL.Health.Infrastructure;
using System;
using System.Threading.Tasks;

namespace VL.Health.API.Managers
{
    public class MedicalControlManager : IMedicalControlManager
    {
        private readonly IMedicalControlRepository _medicalControlRepository;
        private readonly IHelperResultValidator _helperResultValidator;
        private readonly ICustomValidator<MedicalControlTracking> _medicalControlValidator;
        private readonly IWebApiGateway _webApiGateway;

        public MedicalControlManager(IMedicalControlRepository medicalControlRepository, 
                                    IHelperResultValidator repositoryResultValidator,
                                    IWebApiGateway webApiGateway,
                                    ICustomValidator<MedicalControlTracking> medicalControlValidator)
        {
            _medicalControlRepository = medicalControlRepository;
            _helperResultValidator = repositoryResultValidator;
            _medicalControlValidator = medicalControlValidator;
            _webApiGateway = webApiGateway;
        }

        public MedicalControl Get(int id, bool tracking)
        {
            if (tracking)
            {
                return _helperResultValidator.ObjectResult(_medicalControlRepository.GetWithTracking, id);
            }
            return _helperResultValidator.ObjectResult(_medicalControlRepository.Get, id);

        }

        public int Create(MedicalControlTracking medicalControl)
        {
            if (!_medicalControlValidator.IsValid(medicalControl, ActionType.Create))
            {
                throw new FunctionalException(ErrorType.ValidationError, _medicalControlValidator.Errors);
            }

            return _helperResultValidator.IntegerResult(_medicalControlRepository.Create, medicalControl);
        }

        public void Update(MedicalControlTracking medicalControl)
        {
            if (!_medicalControlValidator.IsValid(medicalControl, ActionType.Update))
            {
                throw new FunctionalException(ErrorType.ValidationError, _medicalControlValidator.Errors);
            }
            
            _helperResultValidator.IntegerResult(_medicalControlRepository.Update, medicalControl);
        }

        public async Task<MedicalControlTracking> Delete(int id)
        {
            try {
                var medicalControl = new MedicalControlTracking{ Id = id };
                if (!_medicalControlValidator.IsValid(medicalControl, ActionType.Delete))
                {
                    throw new FunctionalException(ErrorType.ValidationError, _medicalControlValidator.Errors);
                }
                medicalControl = Get(id, false);
                if (medicalControl.IdFile > 0)
                {                    
                    await DeleteFile((int)medicalControl.IdFile, medicalControl.Employee.Id); 
                }
                if (medicalControl.IdFileComplaint > 0)
                {
                    await DeleteFile((int)medicalControl.IdFileComplaint, medicalControl.Employee.Id);
                }
                return _helperResultValidator.ObjectResult<MedicalControlTracking>(_medicalControlRepository.Delete, id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task DeleteFile(int idFile  , int idEmployee)
        {
            DeleteWebApiFileRequest request = new DeleteWebApiFileRequest();
            request.EntityTypeId = EntityType.Employee;
            request.FileId = idFile;
            request.Type = "file";
            request.EntityId = idEmployee;
            await _webApiGateway.Delete(request);
            
        }

        public MedicalControlTracking RectifyAbsence(int oldIdAbsence, int newIdAbsence)
        {
            var medicalControl = _helperResultValidator.ObjectResult(_medicalControlRepository.GetByAbsenceId, oldIdAbsence);
            medicalControl.Absence.Id = newIdAbsence;

            if (!_medicalControlValidator.IsValid(medicalControl, ActionType.Update))
            {
                throw new FunctionalException(ErrorType.ValidationError, _medicalControlValidator.Errors);
            }

            _helperResultValidator.IntegerResult(_medicalControlRepository.UpdateAbsenceId, medicalControl);
            return medicalControl;
        }

        public MedicalControlTracking GetByAbsenceId(int idAbsence)
        {
            var medicalControlTracking = _helperResultValidator.ObjectResult(_medicalControlRepository.GetByAbsenceId, idAbsence);
            return medicalControlTracking;
        }

        public void Patch(int id, JsonPatchDocument operation)
        {
            var medicalControl = _medicalControlRepository.Get(id);
                  
            operation.ApplyTo(medicalControl);

            _helperResultValidator.IntegerResult(_medicalControlRepository.Update, medicalControl);
        }

    }
}
