using Microsoft.Extensions.DependencyInjection;
using VL.Health.API.Managers;
using VL.Health.DB.Repositories;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Helpers;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Interfaces.Managers;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Entities;
using VL.Health.API.Validators;
using VL.Health.Infrastructure.Interfaces;
using VL.Health.Infrastructure;

namespace VL.Health.IoC
{
    public static class HealthIoC
    {
        public static void ConfigureIoC(this IServiceCollection services)
        {
            #region Managers
            services.AddTransient<IHelperResultValidator, HelperResultValidator>();
            services.AddTransient<IAccidentNotificationDischargeChannelManager, AccidentNotificationDischargeChannelManager>();
            services.AddTransient<IAccidentMedicalProviderManager, AccidentMedicalProviderManager>();
            services.AddTransient<IAccidentPathologyManager, AccidentPathologyManager>();
            services.AddTransient<IAccidentComplainantManager, AccidentComplainantManager>();
            services.AddTransient<IAccidentComplaintChannelManager, AccidentComplaintChannelManager>();
            services.AddTransient<IAccidentInsuranceCompanyManager, AccidentInsuranceCompanyManager>();
            services.AddTransient<IAccidentStatusManager, AccidentStatusManager>();
            services.AddTransient<IDoctorManager, DoctorManager>();
            services.AddTransient<IPathologyManager, PathologyManager>();
            services.AddTransient<IMedicalServiceManager, MedicalServiceManager>();
            services.AddTransient<IMedicalControlTypeManager, MedicalControlTypeManager>();
            services.AddTransient<IAccidentReopeningManager, AccidentReopeningManager>();
            services.AddTransient<IAccidentTypeManager, AccidentTypeManager>();
            services.AddTransient<IBloodPressureManager, BloodPressureManager>();
            services.AddTransient<IBloodTypeManager, BloodTypeManager>();
            services.AddTransient<IEmployeeMedicalHistoryManager, EmployeeMedicalHistoryManager>();
            services.AddTransient<IMedicalControlActionManager, MedicalControlActionManager>();
            services.AddTransient<IVaccineManager, VaccineManager>();
            services.AddTransient<IEmployeeVaccineManager, EmployeeVaccineManager>();
            services.AddTransient<IEmployeePathologyManager, EmployeePathologyManager>();
            services.AddTransient<IEmployeeMedicalExamManager, EmployeeMedicalExamManager>();
            services.AddTransient<IMedicalControlManager, MedicalControlManager>();
            services.AddTransient<IMedicalControlTrackingTypeManager, MedicalControlTrackingTypeManager>();
            #endregion

            #region Repositories
            services.AddTransient<IAccidentNotificationDischargeChannelRepository, AccidentNotificationDischargeChannelRepository>();
            services.AddTransient<IAccidentMedicalProviderRepository, AccidentMedicalProviderRepository>();
            services.AddTransient<IAccidentPathologyRepository, AccidentPathologyRepository>();
            services.AddTransient<IAccidentComplainantRepository, AccidentComplainantRepository>();
            services.AddTransient<IAccidentComplaintChannelRepository, AccidentComplaintChannelRepository>();
            services.AddTransient<IAccidentInsuranceCompanyRepository, AccidentInsuranceCompanyRepository>();
            services.AddTransient<IAccidentStatusRepository, AccidentStatusRepository>();
            services.AddTransient<IDoctorRepository, DoctorRepository>();
            services.AddTransient<IPathologyRepository, PathologyRepository>();
            services.AddTransient<IMedicalServiceRepository, MedicalServiceRepository>();
            services.AddTransient<IMedicalControlTypeRepository, MedicalControlTypeRepository>();
            services.AddTransient<IAccidentTypeRepository, AccidentTypeRepository>();
            services.AddTransient<IAccidentReopeningRepository, AccidentReopeningRepository>();
            services.AddTransient<IBloodTypeRepository, BloodTypeRepository>();
            services.AddTransient<IBloodPressureRepository, BloodPressureRepository>();
            services.AddTransient<IEmployeeMedicalHistoryRepository, EmployeeMedicalHistoryRepository>();
            services.AddTransient<IMedicalControlActionRepository, MedicalControlActionRepository>();
            services.AddTransient<IVaccineRepository, VaccineRepository>();
            services.AddTransient<IEmployeeVaccineRepository, EmployeeVaccineRepository>();
            services.AddTransient<IEmployeePathologyRepository, EmployeePathologyRepository>();
            services.AddTransient<IEmployeeMedicalExamRepository, EmployeeMedicalExamRepository>();
            services.AddTransient<IMedicalControlRepository, MedicalControlRepository>();
            services.AddTransient<IMedicalControlTrackingTypeRepository, MedicalControlTrackingTypeRepository>();
            #endregion

            #region Gateways
            services.AddTransient<IPersonGateway, PersonGateway>();
            services.AddTransient<IWebApiGateway, WebApiGateway>();
            services.AddTransient<IAbsenceGateway, AbsenceGateway>();
            #endregion

            #region Validators
            services.AddTransient<ICustomValidator<EmployeeMedicalHistory>, EmployeeMedicalHistoryValidator>();
            services.AddTransient<ICustomValidator<Doctor>, DoctorValidator>();
            services.AddTransient<ICustomValidator<MedicalService>, MedicalServiceValidator>();
            services.AddTransient<ICustomValidator<EmployeeVaccines>, EmployeeVaccinesValidator>();
            services.AddTransient<ICustomValidator<EmployeePathologies>, EmployeePathologiesValidator>();
            services.AddTransient<ICustomValidator<EmployeeMedicalExam>, EmployeeMedicalExamValidator>();
            services.AddTransient<ICustomValidator<MedicalControlTracking>, MedicalControlValidator>();
            services.AddTransient<IHelperResultValidator, HelperResultValidator>();
            #endregion
        }
    }
}
