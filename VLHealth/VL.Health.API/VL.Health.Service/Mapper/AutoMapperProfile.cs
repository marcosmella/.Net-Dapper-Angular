using AutoMapper;
using VL.Health.Domain.Entities;
using VL.Health.Service.DTO.Doctor.Response;
using VL.Health.Service.DTO.Pathology.Response;
using VL.Health.Service.DTO.MedicalService.Response;
using VL.Health.Service.DTO.MedicalControlType.Response;
using VL.Health.Service.DTO.AccidentType.Response;
using VL.Health.Service.DTO.AccidentPathology.Response;
using VL.Health.Service.DTO.AccidentMedicalProvider.Response;
using VL.Health.Service.DTO.AccidentNotificationDischargeChannel.Response;
using VL.Health.Service.DTO.AccidentComplainant.Response;
using VL.Health.Service.DTO.AccidentComplaintChannel.Response;
using VL.Health.Service.DTO.AccidentInsuranceCompany.Response;
using VL.Health.Service.DTO.AccidentStatus.Response;
using VL.Health.Service.DTO.AccidentReopening.Response;
using VL.Health.Service.DTO.BloodPressure.Response;
using VL.Health.Service.DTO.BloodType.Response;
using VL.Health.Service.DTO.Doctor.Request;
using VL.Health.Service.DTO.MedicalControlAction.Response;
using VL.Health.Service.DTO.MedicalService.Request;
using VL.Health.Service.DTO.EmployeeMedicalHistory.Request;
using VL.Health.Service.DTO.EmployeeMedicalHistory.Response;
using VL.Health.Service.DTO.Vaccine.Response;
using VL.Health.Service.DTO.EmployeeVaccine.Response;
using VL.Health.Service.DTO.EmployeeVaccine.Request;
using VL.Health.Service.DTO.EmployeePathology.Request;
using VL.Health.Service.DTO.EmployeePathology.Response;
using VL.Health.Service.DTO.EmployeeMedicalExam.Request;
using VL.Health.Service.DTO.EmployeeMedicalExam.Response;
using VL.Health.Service.DTO.MedicalControlTracking.Request;
using VL.Health.Service.DTO.MedicalControl.Response;
using VL.Health.Domain.Enums;
using System.Linq;

namespace VL.Health.Service.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Request
            CreateMap<EmployeeMedicalHistoryRequest, EmployeeMedicalHistory>();
            CreateMap<DoctorRequest, Doctor>();
            CreateMap<MedicalServiceRequest, MedicalService>();
            CreateMap<EmployeeVaccinesRequest, EmployeeVaccines>();
            CreateMap<EmployeeVaccineRequest, EmployeeVaccine>();
            CreateMap<EmployeePathologiesRequest, EmployeePathologies>();
            CreateMap<EmployeePathologyRequest, EmployeePathology>()
                .ForMember(dest => dest.Description, opt => opt.Ignore());
            CreateMap<EmployeeMedicalExamRequest, EmployeeMedicalExam>();
            CreateMap<MedicalControlRequest, MedicalControlTracking>()
                .ForMember(dest => dest.IdFileComplaint, opt => opt.Ignore())
                .ForPath(dest => dest.Employee.Id, opt => opt.MapFrom(src => src.IdEmployee))
                .ForPath(dest => dest.ControlType.Id, opt => opt.MapFrom(src => src.IdControlType))
                .ForPath(dest => dest.MedicalService.Id, opt => opt.MapFrom(src => src.IdMedicalService))
                .ForMember(dest => dest.BreakTime, opt => opt.Condition(x => x.IdAction == (int)MedicalActionValue.Break))
                .ForMember(dest => dest.TestDate, opt => opt.Condition(x => x.IdAction == (int)MedicalActionValue.PCRTest))
                .ForMember(dest => dest.TestResult, opt => opt.Condition(x => x.IdAction == (int)MedicalActionValue.PCRTest))
                .ForPath(dest => dest.OccupationalDoctor, opt => opt.MapFrom(src => src.IdOccupationalDoctor.HasValue
                                                                                    ? new Doctor { Id = (int)src.IdOccupationalDoctor }
                                                                                    : null))
                .ForPath(dest => dest.Absence, opt => opt.MapFrom(src => src.IdAbsence.HasValue
                                                                                    ? new Absence { Id = (int)src.IdAbsence }
                                                                                    : null))
                .ForPath(dest => dest.Action, opt => opt.MapFrom(src => src.IdAction.HasValue
                                                                                    ? new MedicalControlAction { Id = (int)src.IdAction }
                                                                                    : null))
                .ForPath(dest => dest.TrackingType, opt => opt.MapFrom(src => src.IdTrackingType.HasValue
                                                                                    ? new MedicalControlTrackingType { Id = (int)src.IdTrackingType }
                                                                                    : null))
                .ForMember(dest => dest.Pathologies, opt => opt.MapFrom(
                    src => src.Pathologies.Select(
                            x => new Pathology() { Id = x }).ToList() 
                    )
                );

            #endregion

            #region Response
            CreateMap<AccidentNotificationDischargeChannel, AccidentNotificationDischargeChannelResponse>();
            CreateMap<AccidentMedicalProvider, AccidentMedicalProviderResponse>();
            CreateMap<AccidentPathology, AccidentPathologyResponse>();
            CreateMap<AccidentComplainant, AccidentComplainantResponse>();
            CreateMap<AccidentComplaintChannel, AccidentComplaintChannelResponse>();
            CreateMap<AccidentInsuranceCompany, AccidentInsuranceCompanyResponse>();
            CreateMap<AccidentStatus, AccidentStatusResponse>();
            CreateMap<Doctor, DoctorResponse>();
            CreateMap<Pathology, PathologyResponse>();
            CreateMap<MedicalService, MedicalServiceResponse>();
            CreateMap<MedicalControlType, MedicalControlTypeResponse>();
            CreateMap<AccidentType, AccidentTypeResponse>();
            CreateMap<AccidentReopening, AccidentReopeningResponse>();
            CreateMap<BloodPressure, BloodPressureResponse>();
            CreateMap<BloodType, BloodTypeResponse>();
            CreateMap<EmployeeMedicalHistory, EmployeeMedicalHistoryResponse>();
            CreateMap<MedicalControlAction, MedicalControlActionResponse>();
            CreateMap<Vaccine, VaccineResponse>();
            CreateMap<EmployeeVaccine, EmployeeVaccineResponse>();
            CreateMap<EmployeePathology, EmployeePathologyResponse>();
            CreateMap<EmployeeMedicalExam, EmployeeMedicalExamResponse>();
            CreateMap<MedicalControlTrackingType, MedicalControlTrackingTypeResponse>();
            CreateMap<MedicalControl, MedicalControlResponse>()
                .ForMember(dest => dest.IdEmployee, opt => opt.MapFrom(src => src.Employee.Id))
                .ForMember(dest => dest.IdControlType, opt => opt.MapFrom(src => src.ControlType.Id))
                .ForMember(dest => dest.IdAbsence, opt => opt.MapFrom(src => src.Absence.Id))
                .ForMember(dest => dest.IdAction, opt => opt.MapFrom(src => src.Action.Id))
                .ForMember(dest => dest.IdOccupationalDoctor, opt => opt.MapFrom(src => src.OccupationalDoctor.Id))
                .ForMember(dest => dest.IdMedicalService, opt => opt.MapFrom(src => src.MedicalService.Id))
                .ForMember(dest => dest.IdTrackingType, opt => opt.MapFrom(src => src.TrackingType.Id));

            CreateMap<MedicalControlTracking, MedicalControlTrackingResponse>()
                .ForMember(dest => dest.IdEmployee, opt => opt.MapFrom(src => src.Employee.Id))
                .ForMember(dest => dest.IdControlType, opt => opt.MapFrom(src => src.ControlType.Id))
                .ForMember(dest => dest.IdAbsence, opt => opt.MapFrom(src => src.Absence.Id))
                .ForMember(dest => dest.IdAction, opt => opt.MapFrom(src => src.Action.Id))
                .ForMember(dest => dest.IdOccupationalDoctor, opt => opt.MapFrom(src => src.OccupationalDoctor.Id))
                .ForMember(dest => dest.IdMedicalService, opt => opt.MapFrom(src => src.MedicalService.Id))
                .ForMember(dest => dest.IdTrackingType, opt => opt.MapFrom(src => src.TrackingType.Id));

            #endregion
        }
    }
}
