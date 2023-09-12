using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.Domain.Enums;
using VL.Health.Infrastructure.Interfaces;
using VL.Health.Interfaces.Repositories;

namespace VL.Health.API.Validators
{
    public class MedicalControlValidator : AbstractValidator<MedicalControlTracking>, ICustomValidator<MedicalControlTracking>
    {

        private readonly IMedicalControlRepository _medicalControlRepository;
        private readonly IMedicalServiceRepository _medicalServiceRepository;
        private readonly IMedicalControlActionRepository _medicalControlActionRepository;
        private readonly IMedicalControlTypeRepository _medicalControlTypeRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMedicalControlTrackingTypeRepository _medicalControlTrackingTypeRepository;
        private readonly IPathologyRepository _pathologyRepository;
        private readonly IAbsenceGateway _absenceGateway;
        private ActionType _actionType;

        private MedicalControl _medicalControlParent = null;

        public List<string> Errors { get; private set; }

        public MedicalControlValidator(
            IMedicalControlRepository medicalControlRepository, 
            IMedicalServiceRepository medicalServiceRepository,
            IMedicalControlActionRepository medicalControlActionRepository,
            IMedicalControlTypeRepository medicalControlTypeRepository,
            IDoctorRepository doctorRepository,
            IMedicalControlTrackingTypeRepository medicalControlTrackingTypeRepository,
            IPathologyRepository pathologyRepository,
            IAbsenceGateway absenceGateway
        )
        {
            _medicalControlRepository = medicalControlRepository;
            _medicalServiceRepository = medicalServiceRepository;
            _medicalControlActionRepository = medicalControlActionRepository;
            _medicalControlTypeRepository = medicalControlTypeRepository;
            _doctorRepository = doctorRepository;
            _medicalControlTrackingTypeRepository = medicalControlTrackingTypeRepository;
            _pathologyRepository = pathologyRepository;
            _absenceGateway = absenceGateway;
        }

        public bool IsValid(MedicalControlTracking medicalControl, ActionType actionType)
        {
            _actionType = actionType;

            if (medicalControl.IdParent!=null)
            {
                _medicalControlParent = _medicalControlRepository.Get((int)medicalControl.IdParent);
            }
            
            Validations();

            var validationResult = base.Validate(medicalControl);

            Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();

            return validationResult.IsValid;
        }

        public void Validations()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithErrorCode("IdIsRequired")
                .WithMessage("IdIsRequired")
                .When(x => _actionType == ActionType.Update);

            RuleFor(x => x.Employee.Id)
                .NotEmpty()
                .WithErrorCode("IdEmployeeIsRequired")
                .WithMessage("EmployeeIsRequired")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x.ControlType.Id)
                .NotEmpty()
                .WithErrorCode("IdControlTypeIsRequired")
                .WithMessage("ControlTypeIsRequired")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x.BreakTime)
                .NotEmpty()
                .WithErrorCode("BreakTimeIsRequired")
                .ExclusiveBetween(0, 61)
                .WithErrorCode("BreakTimeInvalid")
                .WithMessage("BreakTimeInvalid")
                .Must(x => (x % 5) == 0)
                .WithErrorCode("BreakTimeInvalid")
                .WithMessage("BreakTimeInvalid")
                .When(x => (x.Action != null && x.Action.Id == (int)MedicalActionValue.Break) && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.TestResult)
                .NotEmpty()
                .WithErrorCode("TestResultRequired")
                .WithMessage("TestResultRequired")
                .When(x => (x.Action != null && x.Action.Id == (int)MedicalActionValue.PCRTest) && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.TestDate)
                .NotEmpty()
                .WithErrorCode("TestDateRequired")
                .WithMessage("TestDateRequired")
                .When(x => (x.Action != null && x.Action.Id == (int)MedicalActionValue.PCRTest) && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.Action)
                .NotEmpty()
                .WithErrorCode("IdActionIsRequired")
                .WithMessage("IdActionIsRequired")
                .When(x => (!x.IdParent.HasValue) && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.TrackingType)
                .Empty()
                .WithErrorCode("TrackingTypeNotNeededForMainControl")
                .WithMessage("TrackingTypeNotNeededForMainControl")
                .When(x => (!x.IdParent.HasValue) && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.TrackingType)
                .NotEmpty()
                .WithErrorCode("TrackingTypeRequiredForTrackingControls")
                .WithMessage("TrackingTypeRequiredForTrackingControls")
                .When(x => x.IdParent.HasValue && (_actionType == ActionType.Create || _actionType == ActionType.Update));


            #region DataBase Validator    

            RuleFor(x => x)
                .Must(x => !_medicalControlRepository.AbsenceRelationshipExists(x.Id, x.Absence.Id))
                .WithMessage("AbsenceAlreadyRelated")
                .WithErrorCode("AbsenceAlreadyRelated")
                .When(x => x.Absence != null && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.MedicalService)
                .Must(x => _medicalServiceRepository.Exists(x.Id))
                .WithMessage("MedicalServiceNotExists")
                .WithErrorCode("MedicalServiceNotExists")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x.Action)
                .Must(x => _medicalControlActionRepository.Exists(x.Id))
                .WithMessage("ActionNotExists")
                .WithErrorCode("ActionNotExists")
                .When(x => x.IdParent == null && x.Action != null && _actionType == ActionType.Create);

            RuleFor(x => x.OccupationalDoctor.Id)
                .Must(x => _doctorRepository.Exists(x))
                .WithMessage("OccupationalDoctorNotExists")
                .WithErrorCode("OccupationalDoctorNotExists")
                .When(x => x.OccupationalDoctor != null && x.OccupationalDoctor.Id > 0 && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.PrivateDoctorName)
                .Empty()
                .WithMessage("CannotDefinePrivateDoctorNameAndOccupationalDoctor")
                .WithErrorCode("CannotDefinePrivateDoctorNameAndOccupationalDoctor")
                .When(x => x.OccupationalDoctor != null && x.OccupationalDoctor.Id > 0 && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.PrivateDoctorEnrollment)
                .Empty()
                .WithMessage("CannotDefinePrivateDoctorEnrollmentAndOccupationalDoctor")
                .WithErrorCode("CannotDefinePrivateDoctorEnrollmentAndOccupationalDoctor")
                .When(x => x.OccupationalDoctor != null && x.OccupationalDoctor.Id > 0 && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.PrivateDoctorName)
                .NotEmpty()
                .WithMessage("ShouldDefinePrivateDoctorNameOrOccupationalDoctor")
                .WithErrorCode("ShouldDefinePrivateDoctorNameOrOccupationalDoctor")
                .When(x => (x.OccupationalDoctor == null || x.OccupationalDoctor.Id == 0) && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.PrivateDoctorEnrollment)
                .NotEmpty()
                .WithMessage("ShouldDefinePrivateDoctorEnrollmentOrOccupationalDoctor")
                .WithErrorCode("ShouldDefinePrivateDoctorEnrollmentOrOccupationalDoctor")
                .When(x => (x.OccupationalDoctor == null || x.OccupationalDoctor.Id == 0) && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.ControlType)
                .Must(x => _medicalControlTypeRepository.Exists(x.Id))
                .WithMessage("MedicalControlTypeNotExists")
                .WithErrorCode("MedicalControlTypeNotExists")
                .When(x => _actionType == ActionType.Create || _actionType == ActionType.Update);

            RuleFor(x => x).Cascade(CascadeMode.StopOnFirstFailure)
                .Must(x => x.IdParent != 0)
                .WithMessage("ParentIdInvalid")
                .WithErrorCode("ParentIdInvalid")
                .Must(x => _medicalControlRepository.Exists(x.IdParent ?? 0))
                .WithMessage("ParentNotExists")
                .WithErrorCode("ParentNotExists")
                .Must(x => _medicalControlRepository.TrackingDateValid(x))
                .WithMessage("TrackingDateInvalid")
                .WithErrorCode("TrackingDateInvalid")
                .Must(x => x.ControlType.Id != (int)ControlTypeValue.WorkAccident)
                .WithMessage("InvalidControlTypeForTracking")
                .WithErrorCode("InvalidControlTypeForTracking")
                .When(x => x.IdParent != null && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.TrackingType)
                .Must(x => _medicalControlTrackingTypeRepository.Get().Exists(tt => tt.Id == x.Id && tt.CreateAbsence))
                .WithMessage("AbsenceNotNeededForTrackingType")
                .WithErrorCode("AbsenceNotNeededForTrackingType")
                .When(x => x.IdParent != null && x.TrackingType != null && x.Absence != null && _actionType == ActionType.Create);

            RuleFor(x => x.TrackingType)
                .Must(x => _medicalControlTrackingTypeRepository.Get().Exists(tt => tt.Id == x.Id && !tt.CreateAbsence))
                .WithMessage("AbsenceNeededForTrackingType")
                .WithErrorCode("AbsenceNeededForTrackingType")
                .When(x => x.IdParent != null && x.TrackingType != null && x.Absence == null && _actionType == ActionType.Create);

            RuleFor(x => x.Action)
                .Must(x => _medicalControlActionRepository.Get().Exists(action => action.Id == x.Id && action.CreateAbsence))
                .WithMessage("AbsenceNotNeededForAction")
                .WithErrorCode("AbsenceNotNeededForAction")
                .When(x => x.IdParent == null && x.Action != null && x.Absence != null && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.Action)
                .Must(x => _medicalControlActionRepository.Get().Exists(action => action.Id == x.Id && !action.CreateAbsence))
                .WithMessage("AbsenceNeededForAction")
                .WithErrorCode("AbsenceNeededForAction")
                .When(x => x.IdParent == null && x.Action != null && x.Absence == null && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x.Action) 
                .Empty()
                .WithMessage("ActionNotNeededForTrackingControl")
                .WithErrorCode("ActionNotNeededForTrackingControl")
                .When(x => x.IdParent != null && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x)
                .Must(x => _medicalControlRepository.IsParentOrLastControlTracking(x.Id))
                .WithMessage("OnlyLastControlTrackingCanBeDeleted")
                .WithErrorCode("OnlyLastControlTrackingCanBeDeleted")
                .When(x => _actionType == ActionType.Delete);

            RuleFor(x => x)
                .Must(x => LastTrackingOfParentIsMedicalRelease((int)x.IdParent))
                .WithMessage("PreviousTrackingTypeShouldBeMedicalRelease")
                .WithErrorCode("PreviousTrackingTypeShouldBeMedicalRelease")
                .When(x => x.TrackingType != null && x.TrackingType.Id == (int)TrackingTypeValue.ReOpening && x.IdParent != null && _actionType == ActionType.Create);

            RuleFor(x => x).Cascade(CascadeMode.StopOnFirstFailure)
                .Must(x => _medicalControlRepository.Exists(x.IdParent ?? 0))
                .WithMessage("ParentNotExists")
                .WithErrorCode("ParentNotExists")
                .Must(x => _medicalControlParent.Absence != null)
                .WithMessage("ParentDoesNotAllowTrackingControls")
                .WithErrorCode("ParentDoesNotAllowTrackingControls")
                .When(x => x.IdParent != null && _medicalControlParent != null && x.TrackingType != null 
                    && _medicalControlParent.ControlType.Id != (int)ControlTypeValue.AccidentComplaint 
                    && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x)
                .Must(x => _pathologyRepository.ExistsAll(x.Pathologies.Select(x => x.Id).ToArray()))
                .WithMessage("PathologyNotExists")
                .WithErrorCode("PathologyNotExists")
                .When(x => (_actionType == ActionType.Create || _actionType == ActionType.Update) && x.Pathologies.Count > 0);

            #endregion

            #region Accident Complaint
            RuleFor(x => x)
                .Must(x => _medicalControlRepository.Get((int)x.IdParent).ControlType.Id == (int)ControlTypeValue.AccidentComplaint)
                .WithMessage("RejectionTrackingTypeIsNotAllow")
                .WithErrorCode("RejectionTrackingTypeIsNotAllow")
                .When(x => x.IdParent != null 
                    && (x.TrackingType.Id == (int)TrackingTypeValue.Rejection) 
                    && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x)
                .Must(x => _medicalControlRepository.Get((int)x.IdParent).ControlType.Id == (int)ControlTypeValue.AccidentComplaint)
                .WithMessage("GenerationAbsenceTrackingTypeIsNotAllow")
                .WithErrorCode("GenerationAbsenceTrackingTypeIsNotAllow")
                .When(x => x.IdParent != null 
                    && (x.TrackingType.Id == (int)TrackingTypeValue.GenerationAbsence) 
                    && (_actionType == ActionType.Create || _actionType == ActionType.Update));
                       
            RuleFor(x => x)
                 .Must(x =>  _medicalControlRepository.Get((int)x.IdParent).ControlType.Id == (int)ControlTypeValue.AccidentComplaint 
                 && !ExistTrackingOfType(x, TrackingTypeValue.GenerationAbsence))
                 .WithMessage("ComplaintHasAbsence")
                 .WithErrorCode("ComplaintHasAbsence")
                 .When(x => x.IdParent != null
                    && (x.TrackingType.Id == (int)TrackingTypeValue.Rejection || x.TrackingType.Id == (int)TrackingTypeValue.GenerationAbsence)
                    && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            RuleFor(x => x)
               .Must(x => _medicalControlRepository.Get((int)x.IdParent).ControlType.Id == (int)ControlTypeValue.AccidentComplaint
                && !ExistTrackingOfType(x, TrackingTypeValue.Rejection))
               .WithMessage("ComplaintHasRejection")
               .WithErrorCode("ComplaintHasRejection")
               .When(x => x.IdParent != null && x.TrackingType.Id == (int)TrackingTypeValue.Rejection && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            #endregion

            #region Absence Validator

            RuleFor(x => x)
                .Must(x => AbsenceTypeIsValid(x))
                .WithMessage("AbsenceTypeNotValid")
                .WithErrorCode("AbsenceTypeNotValid")
                .Must(x => IsDateInAbsencePeriod(x))
                .WithMessage("DateNotValidForRelatedAbsence")
                .WithErrorCode("DateNotValidForRelatedAbsence")
                .When(x => x.Absence != null && (_actionType == ActionType.Create || _actionType == ActionType.Update));

            #endregion

        }

        #region Private Method
        bool ExistTrackingOfType(MedicalControlTracking medicalControlTracking, TrackingTypeValue trackingType)
        {
            var medicalControlParent = _medicalControlRepository.GetWithTracking((int)medicalControlTracking.IdParent);
            if (medicalControlParent.Tracking != null)
            {
                return medicalControlParent.Tracking.Exists(tracking => tracking.TrackingType.Id == (int)trackingType);
            };

            return false;
        }

        private bool LastTrackingOfParentIsMedicalRelease(int idParent)
        {
            var parentMedicalControl = _medicalControlRepository.GetWithTracking(idParent);
            return (parentMedicalControl != null
                    && parentMedicalControl.Tracking.Count > 0
                        && parentMedicalControl.Tracking.First().TrackingType.Id == (int)TrackingTypeValue.MedicalRelease);

        }
     
        
        private bool AbsenceTypeIsValid(MedicalControlTracking medicalControlTracking)
        {
            bool result = false;
            var absence = _absenceGateway.GetAbsence(medicalControlTracking.Absence.Id);
            if (absence != null)
            {
                var absenceType = _absenceGateway.GetAbsenceType(absence.IdAbsenceType);
                result = (absenceType == null) ? false : absenceType.OccupationalHealth;
            }
            return result;
        }


        private bool IsDateInAbsencePeriod(MedicalControlTracking medicalControlTracking)
        {
            bool result = false;
            var absence = _absenceGateway.GetAbsence(medicalControlTracking.Absence.Id);
            if (absence != null)
            {
                result = (absence.DateFrom.Date <= medicalControlTracking.Date.Date) && (absence.DateTo.Date >= medicalControlTracking.Date.Date);
            }
            return result;
        }

        #endregion
    }
}
