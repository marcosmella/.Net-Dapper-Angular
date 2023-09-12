using AutoMoqCore;
using System;
using VL.Health.API.Managers;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using Xunit;
using VL.Health.API.Helpers.Interfaces;
using Moq;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Enums;
using System.Collections.Generic;
using VL.Health.Interfaces.Repositories;
using VL.Health.API.Exceptions;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using VL.Health.Infrastructure;
using VL.Health.Infrastructure.DTO.WebApiFile;
using System.Threading.Tasks;

namespace VL.Health.API.Tests.Managers
{
    public class MedicalControlManagerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly IMedicalControlManager _medicalControlManager;

        public MedicalControlManagerTests()
        {
            _autoMoqer = new AutoMoqer();
            _medicalControlManager = _autoMoqer.Resolve<MedicalControlManager>();
        }

        [Fact]
        public void GetMedicalControlWithTracking()
        {
            //ARRANGE
            int idMedicalControlParent = 1;
            int idMedicalControlChild = 2;

            var medicalControlTracking = new MedicalControlTracking()
            {
                Id = idMedicalControlChild,
                Employee = new Employee { Id = 193 },
                ControlType = new MedicalControlType { Id = 1 },
                Action = new MedicalControlAction { Id = 1 },
                Date = DateTime.Now,
                MedicalService = new MedicalService { Id = 1 },
                OccupationalDoctor = new Doctor { Id = 24 },
                Absence = new Absence { Id = 250 },
                Diagnosis = "Lumbalgia",
                BreakTime = 5,
                PrivateDoctorEnrollment = "",
                PrivateDoctorName = "",
                IdFile = 123,
                TestDate = DateTime.Now,
                TestResult = true,
                IdParent = idMedicalControlParent
            };

            var medicalControlWithoutChild = new MedicalControl
            {
                Id = idMedicalControlParent,
                Employee = new Employee { Id = 193 },
                ControlType = new MedicalControlType { Id = 1 },
                Action = new MedicalControlAction { Id = 1 },
                Date = DateTime.Now,
                MedicalService = new MedicalService { Id = 1 },
                OccupationalDoctor = new Doctor { Id = 24 },
                Absence = new Absence { Id = 250 },
                Diagnosis = "Lumbalgia",
                BreakTime = 5,
                PrivateDoctorEnrollment = "",
                PrivateDoctorName = "",
                IdFile = 123,
                TestDate = DateTime.Now,
                TestResult = true,
                Tracking = new List<MedicalControlTracking>()
            };

            var medicalControlWithChild = new MedicalControl
            {
                Id = idMedicalControlParent,
                Employee = new Employee { Id = 193 },
                ControlType = new MedicalControlType { Id = 1 },
                Action = new MedicalControlAction { Id = 1 },
                Date = DateTime.Now,
                MedicalService = new MedicalService { Id = 1 },
                OccupationalDoctor = new Doctor { Id = 24 },
                Absence = new Absence { Id = 250 },
                Diagnosis = "Lumbalgia",
                BreakTime = 5,
                PrivateDoctorEnrollment = "",
                PrivateDoctorName = "",
                IdFile = 123,
                TestDate = DateTime.Now,
                TestResult = true,
                Tracking = new List<MedicalControlTracking>() {
                    medicalControlTracking
                }
            };

            var mockMedicalControlRepositoryValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockMedicalControlRepositoryValidator.Setup(x => x.ObjectResult(It.IsAny<Func<int, MedicalControl>>(), idMedicalControlParent)).Returns(medicalControlWithChild);

            //ACT
            var result = _medicalControlManager.Get(idMedicalControlParent, true);

            //ASSERT
            Assert.Equal(result.Id, medicalControlWithChild.Id);
            Assert.Equal(result.Employee.Id, medicalControlWithChild.Employee.Id);
            Assert.Equal(result.ControlType.Id, medicalControlWithChild.ControlType.Id);
            Assert.Equal(result.Action.Id, medicalControlWithChild.Action.Id);
            Assert.Equal(result.Date, medicalControlWithChild.Date);
            Assert.Equal(result.Absence.Id, medicalControlWithChild.Absence.Id);
            Assert.Equal(result.OccupationalDoctor.Id, medicalControlWithChild.OccupationalDoctor.Id);
            Assert.Equal(result.Diagnosis, medicalControlWithChild.Diagnosis);
            Assert.Equal(result.BreakTime, medicalControlWithChild.BreakTime);
            Assert.Equal(result.PrivateDoctorEnrollment, medicalControlWithChild.PrivateDoctorEnrollment);
            Assert.Equal(result.PrivateDoctorName, medicalControlWithChild.PrivateDoctorName);
            Assert.Equal(result.IdFile, medicalControlWithChild.IdFile);
            Assert.Equal(result.TestDate, medicalControlWithChild.TestDate);
            Assert.Equal(result.TestResult, medicalControlWithChild.TestResult);
            Assert.Equal(result.Tracking, medicalControlWithChild.Tracking);
        }

        [Fact]
        public void GetMedicalControlWithoutTracking()
        {
            //ARRANGE
            int idMedicalControlParent = 1;
            int idMedicalControlChild = 2;

            var medicalControlTracking = new MedicalControlTracking()
            {
                Id = idMedicalControlChild,
                Employee = new Employee { Id = 193 },
                ControlType = new MedicalControlType { Id = 1 },
                Action = new MedicalControlAction { Id = 1 },
                Date = DateTime.Now,
                MedicalService = new MedicalService { Id = 1 },
                OccupationalDoctor = new Doctor { Id = 24 },
                Absence = new Absence { Id = 250 },
                Diagnosis = "Lumbalgia",
                BreakTime = 5,
                PrivateDoctorEnrollment = "",
                PrivateDoctorName = "",
                IdFile = 123,
                TestDate = DateTime.Now,
                TestResult = true,
                IdParent = idMedicalControlParent
            };

            var medicalControlWithoutChild = new MedicalControl
            {
                Id = idMedicalControlParent,
                Employee = new Employee { Id = 193 },
                ControlType = new MedicalControlType { Id = 1 },
                Action = new MedicalControlAction { Id = 1 },
                Date = DateTime.Now,
                MedicalService = new MedicalService { Id = 1 },
                OccupationalDoctor = new Doctor { Id = 24 },
                Absence = new Absence { Id = 250 },
                Diagnosis = "Lumbalgia",
                BreakTime = 5,
                PrivateDoctorEnrollment = "",
                PrivateDoctorName = "",
                IdFile = 123,
                TestDate = DateTime.Now,
                TestResult = true,
                Tracking = new List<MedicalControlTracking>()
            };

            var medicalControlWithChild = new MedicalControl
            {
                Id = idMedicalControlParent,
                Employee = new Employee { Id = 193 },
                ControlType = new MedicalControlType { Id = 1 },
                Action = new MedicalControlAction { Id = 1 },
                Date = DateTime.Now,
                MedicalService = new MedicalService { Id = 1 },
                OccupationalDoctor = new Doctor { Id = 24 },
                Absence = new Absence { Id = 250 },
                Diagnosis = "Lumbalgia",
                BreakTime = 5,
                PrivateDoctorEnrollment = "",
                PrivateDoctorName = "",
                IdFile = 123,
                TestDate = DateTime.Now,
                TestResult = true,
                Tracking = new List<MedicalControlTracking>() {
                    medicalControlTracking
                }
            };

            var mockMedicalControlRepositoryValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockMedicalControlRepositoryValidator.Setup(x => x.ObjectResult(It.IsAny<Func<int, MedicalControl>>(), idMedicalControlParent)).Returns(medicalControlWithoutChild);

            //ACT
            var result = _medicalControlManager.Get(idMedicalControlParent, false);

            //ASSERT
            Assert.Equal(result.Id, medicalControlWithoutChild.Id);
            Assert.Equal(result.Employee.Id, medicalControlWithoutChild.Employee.Id);
            Assert.Equal(result.ControlType.Id, medicalControlWithoutChild.ControlType.Id);
            Assert.Equal(result.Action.Id, medicalControlWithoutChild.Action.Id);
            Assert.Equal(result.Date, medicalControlWithoutChild.Date);
            Assert.Equal(result.Absence.Id, medicalControlWithoutChild.Absence.Id);
            Assert.Equal(result.OccupationalDoctor.Id, medicalControlWithoutChild.OccupationalDoctor.Id);
            Assert.Equal(result.Diagnosis, medicalControlWithoutChild.Diagnosis);
            Assert.Equal(result.BreakTime, medicalControlWithoutChild.BreakTime);
            Assert.Equal(result.PrivateDoctorEnrollment, medicalControlWithoutChild.PrivateDoctorEnrollment);
            Assert.Equal(result.PrivateDoctorName, medicalControlWithoutChild.PrivateDoctorName);
            Assert.Equal(result.IdFile, medicalControlWithoutChild.IdFile);
            Assert.Equal(result.TestDate, medicalControlWithoutChild.TestDate);
            Assert.Equal(result.TestResult, medicalControlWithoutChild.TestResult);
            Assert.Equal(result.Tracking, medicalControlWithoutChild.Tracking);
        }


        [Fact]
        public void Create()
        {
            //ARRANGE
            var idMedicalControl = 555;
            var medicalControl = new MedicalControlTracking
            {
                Id = 0,
                Employee = new Employee { Id = 193 },
                ControlType = new MedicalControlType { Id = 1 },
                Action = new MedicalControlAction { Id = 1 },
                Date = DateTime.Now,
                MedicalService = new MedicalService { Id = 1 },
                OccupationalDoctor = new Doctor { Id = 24 },
                Absence = new Absence { Id = 250 },
                Diagnosis = "Lumbalgia",
                BreakTime = 5,
                PrivateDoctorEnrollment = "",
                PrivateDoctorName = "",
                IdFile = 123,
                TestDate = DateTime.Now,
                TestResult = true
            };

            bool throwException = true;

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.IntegerResult(It.IsAny<Func<MedicalControlTracking, int>>(), medicalControl, throwException))
                .Returns(idMedicalControl);

            var mockMedicalControlValidator = _autoMoqer.GetMock<ICustomValidator<MedicalControlTracking>>();
            mockMedicalControlValidator.Setup(x => x.IsValid(medicalControl, ActionType.Create)).Returns(true);
            
            //ACT
            var result = _medicalControlManager.Create(medicalControl);

            //ASSERT
            Assert.Equal(idMedicalControl, result);
            mockRepositoryResultValidator.Verify(x => x.IntegerResult(It.IsAny<Func<MedicalControlTracking, int>>(), medicalControl, throwException), Times.Once());
        }


        [Fact]
        public void Update()
        {
            //ARRANGE
            var idMedicalControl = 555;

            var medicalControl = new MedicalControlTracking
            {
                Id = idMedicalControl,
                Employee = new Employee { Id = 193 },
                ControlType = new MedicalControlType { Id = 1 },
                Action = new MedicalControlAction { Id = 1 },
                Date = DateTime.Now,
                MedicalService = new MedicalService { Id = 1 },
                OccupationalDoctor = new Doctor { Id = 24 },
                Absence = new Absence { Id = 250 },
                Diagnosis = "Lumbalgia",
                BreakTime = 5,
                PrivateDoctorEnrollment = "",
                PrivateDoctorName = "",
                IdFile = 123,
                TestDate = DateTime.Now,
                TestResult = true
            };

            bool throwException = true;

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.IntegerResult(It.IsAny<Func<MedicalControlTracking, int>>(), medicalControl, throwException)).Returns(idMedicalControl);

            var mockMedicalControlValidator = _autoMoqer.GetMock<ICustomValidator<MedicalControlTracking>>();
            mockMedicalControlValidator.Setup(x => x.IsValid(medicalControl, ActionType.Update)).Returns(true);

            //ACT
            _medicalControlManager.Update(medicalControl);

            //ASSERT
            mockRepositoryResultValidator.Verify(x => x.IntegerResult(It.IsAny<Func<MedicalControlTracking, int>>(), medicalControl, throwException), Times.Once());
        }

        [Fact]
        public void Delete()
        {
            //ARRANGE
            var idMedicalControl = 555;
            ActionType actionType = ActionType.Create;
            var medicalControl = new MedicalControl
            {
                Employee = new Employee { Id = 123},
                Id = idMedicalControl,
                IdFile = 100,
                IdFileComplaint =120
            };
            var deleteWebApiFile = new DeleteWebApiFileRequest()
            {
                EntityId = 123,
                EntityTypeId = EntityType.Employee,
                FileId = 100,
                Type = "file"
            };
            var mockWorkElementvalidator = _autoMoqer.GetMock<ICustomValidator<MedicalControlTracking>>();
            mockWorkElementvalidator.Setup(x => x.IsValid(medicalControl, actionType))
                .Returns(true);

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.ObjectResult(It.IsAny<Func<int, MedicalControlTracking>>(), idMedicalControl)).Returns(medicalControl);            
            mockRepositoryResultValidator.Setup(x => x.ObjectResult(It.IsAny<Func<int, MedicalControl>>(), idMedicalControl)).Returns(medicalControl);

            var mockMedicalControlValidator = _autoMoqer.GetMock<ICustomValidator<MedicalControlTracking>>();
            mockMedicalControlValidator.Setup(x => x.IsValid(It.IsAny<MedicalControlTracking>(), ActionType.Delete)).Returns(true);

            var mockWebApiGateway = _autoMoqer.GetMock<IWebApiGateway>();
            mockWebApiGateway.Setup(x => x.Delete(deleteWebApiFile));

            //ACT
            _medicalControlManager.Delete(idMedicalControl);

            //ASSERT
            mockRepositoryResultValidator.Verify(x => x.ObjectResult(It.IsAny<Func<int, MedicalControlTracking>>(), idMedicalControl), Times.Once());
        }


        [Fact]
        public void RectifyAbsence()
        {
            //ARRANGE
            var idMedicalControl = 555;
            var oldAbsenceId = 124;
            var newAbsenceId = 243;

            var medicalControl = new MedicalControlTracking
            {
                Id = idMedicalControl,
                Employee = new Employee { Id = 193 },
                ControlType = new MedicalControlType { Id = 1 },
                Action = new MedicalControlAction { Id = 1 },
                Date = DateTime.Now,
                MedicalService = new MedicalService { Id = 1 },
                OccupationalDoctor = new Doctor { Id = 24 },
                Absence = new Absence { Id = 124 },
                Diagnosis = "Lumbalgia",
                IdFile = 123,
                TestDate = DateTime.Now,
                TestResult = true
            };

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.IntegerResult(It.IsAny<Func<MedicalControlTracking, int>>(), medicalControl, true)).Returns(1);
            mockRepositoryResultValidator.Setup(x => x.ObjectResult<MedicalControlTracking>(It.IsAny<Func<int, MedicalControlTracking>>(), oldAbsenceId)).Returns(medicalControl);

            var mockMedicalControlValidator = _autoMoqer.GetMock<ICustomValidator<MedicalControlTracking>>();
            mockMedicalControlValidator.Setup(x => x.IsValid(medicalControl, ActionType.Update)).Returns(true);

            //ACT
            _medicalControlManager.RectifyAbsence(oldAbsenceId, newAbsenceId);

            //ASSERT
            mockRepositoryResultValidator.Verify(x => x.IntegerResult(It.IsAny<Func<MedicalControlTracking, int>>(), medicalControl, true), Times.Once());
        }


        [Fact]
        public void RectifyAbsenceThrowsFunctionalException()
        {
            //ARRANGE
            var idMedicalControl = 555;
            var oldAbsenceId = 124;
            var newAbsenceId = 243;
            var message = "ERROR";

            var medicalControl = new MedicalControlTracking
            {
                Id = idMedicalControl,
                Employee = new Employee { Id = 193 },
                ControlType = new MedicalControlType { Id = 1 },
                Action = new MedicalControlAction { Id = 1 },
                Date = DateTime.Now,
                MedicalService = new MedicalService { Id = 1 },
                OccupationalDoctor = new Doctor { Id = 24 },
                Absence = new Absence { Id = 124 },
                Diagnosis = "Lumbalgia",
                IdFile = 123,
                TestDate = DateTime.Now,
                TestResult = true
            };

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.ObjectResult<MedicalControlTracking>(It.IsAny<Func<int, MedicalControlTracking>>(), oldAbsenceId))
                .Returns(medicalControl);

            var mockMedicalControlValidator = _autoMoqer.GetMock<ICustomValidator<MedicalControlTracking>>();
            mockMedicalControlValidator.Setup(x => x.IsValid(medicalControl, ActionType.Update))
                .Throws(new FunctionalException(ErrorType.ValidationError, message));

            //ACT
            try
            {
                _medicalControlManager.RectifyAbsence(oldAbsenceId, newAbsenceId);
            }
            catch (FunctionalException ex)
            {
                //ASSERT
                Assert.Equal(message, ex.Errors.First());
                Assert.Equal(ErrorType.ValidationError, ex.FunctionalError);
            }

        }

        [Fact]
        public void RectifyAbsenceThrowsNotFoundException()
        {
            //ARRANGE
            var oldAbsenceId = 124;
            var newAbsenceId = 243;

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.ObjectResult<MedicalControlTracking>(It.IsAny<Func<int, MedicalControlTracking>>(), oldAbsenceId))
                .Throws(new FunctionalException(ErrorType.NotFound, ""));


            try
            {
                //ACT
                _medicalControlManager.RectifyAbsence(oldAbsenceId, newAbsenceId);
            }
            catch (FunctionalException ex)
            {
                //ASSERT
                Assert.Equal(ErrorType.NotFound, ex.FunctionalError);
            }

        }

        [Fact]
        public void GetMedicalControlByAbsenceId()
        {
            //ARRANGE
            int idAbsence = 101;
            int idMedicalControl = 2;

            var medicalControl = new MedicalControlTracking()
            {
                Id = idMedicalControl,
                Employee = new Employee { Id = 193 },
                ControlType = new MedicalControlType { Id = 1 },
                Action = new MedicalControlAction { Id = 1 },
                Date = DateTime.Now,
                MedicalService = new MedicalService { Id = 1 },
                OccupationalDoctor = new Doctor { Id = 24 },
                Absence = new Absence { Id = idAbsence },
                Diagnosis = "Lumbalgia",
                IdFile = 123
            };

            var mockMedicalControlRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockMedicalControlRepositoryResultValidator.Setup(x => x.ObjectResult(It.IsAny<Func<int, MedicalControlTracking>>(), idAbsence)).Returns(medicalControl);

            //ACT
            var result = _medicalControlManager.GetByAbsenceId(idAbsence);

            //ASSERT
            Assert.Equal(result.Id, medicalControl.Id);
            Assert.Equal(result.Employee.Id, medicalControl.Employee.Id);
            Assert.Equal(result.ControlType.Id, medicalControl.ControlType.Id);
            Assert.Equal(result.Action.Id, medicalControl.Action.Id);
            Assert.Equal(result.Date, medicalControl.Date);
            Assert.Equal(result.Absence.Id, medicalControl.Absence.Id);
            Assert.Equal(result.OccupationalDoctor.Id, medicalControl.OccupationalDoctor.Id);
            Assert.Equal(result.Diagnosis, medicalControl.Diagnosis);
            Assert.Equal(result.IdFile, medicalControl.IdFile);
        }

        [Fact]
        public void GetMedicalControlByAbsenceIdThrowsNotFoundException()
        {
            //ARRANGE
            int idAbsence = 101;

            var mockMedicalControlRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockMedicalControlRepositoryResultValidator.Setup(x => x.ObjectResult(It.IsAny<Func<int, MedicalControlTracking>>(), idAbsence))
                .Throws(new FunctionalException(ErrorType.NotFound, ""));

            try
            {
                //ACT
                var result = _medicalControlManager.GetByAbsenceId(idAbsence);
            }
            catch (FunctionalException ex)
            {
                //ASSERT
                Assert.Equal(ErrorType.NotFound, ex.FunctionalError);
            }
        }

        [Fact]
        public void Patch()
        {
            //ARRANGE
            var Id = 1;
            var IdFile = 120;
            var operation = new JsonPatchDocument();

            operation.Replace("/IdFileComplaint", IdFile);

            var medicalControl = new MedicalControl
            {
                Id = Id,
                IdFileComplaint = null
            };

            var patchMedicalControl = new MedicalControl
            {
                Id = Id,
                IdFileComplaint = IdFile
            };

            bool throwException = false;

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.IntegerResult(It.IsAny<Func<MedicalControlTracking, int>>(), patchMedicalControl, throwException)).Returns(1);


            var mockMedicalControlRepository = _autoMoqer.GetMock<IMedicalControlRepository>();
            mockMedicalControlRepository.Setup(x => x.Get(Id)).Returns(medicalControl);

            //ACT
            _medicalControlManager.Patch(Id, operation);

            //ASSERT
            mockMedicalControlRepository.Verify(x => x.Get(Id));

        }

        [Fact]
        public void PatchAllMustThrowFunctionalException()
        {
            //ARRANGE
            var Id = 1;
            var IdFile = 120;
            var operation = new JsonPatchDocument();

            operation.Replace("/IdFileComplaint", IdFile);

            var medicalControl = new MedicalControl
            {
                Id = Id,
                IdFileComplaint = null
            };

            var patchMedicalControl = new MedicalControl
            {
                Id = Id,
                IdFileComplaint = IdFile
            };

            bool throwException = true;

            var mockRepositoryResultValidator = _autoMoqer.GetMock<IHelperResultValidator>();
            mockRepositoryResultValidator.Setup(x => x.IntegerResult(It.IsAny<Func<MedicalControlTracking, int>>(), medicalControl, throwException)).Throws(new FunctionalException(ErrorType.NotFound, ""));

            try
            {
                //ACT
                var result = _medicalControlManager.GetByAbsenceId(Id);
            }
            catch (FunctionalException ex)
            {
                //ASSERT
                Assert.Equal(ErrorType.NotFound, ex.FunctionalError);
            }
        }
    }
}
