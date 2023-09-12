using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Service.Controllers;
using Xunit;
using System;
using VL.Health.Service.DTO.MedicalControlTracking.Request;
using System.Collections.Generic;
using VL.Health.Service.DTO.MedicalControl.Response;
using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;

namespace VL.Health.Service.Tests.Controllers
{
    public class MedicalControlControllerTest
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly MedicalControlController _medicalControlController;

        public MedicalControlControllerTest()
        {
            _autoMoqer = new AutoMoqer();
            _medicalControlController = _autoMoqer.Resolve<MedicalControlController>();
        }

        [Fact]
        public void GetMedicalControlWithoutTracking()
        {
            //ARRANGE
            int idMedicalControlParent = 1;
            int idMedicalControlChild = 2;

            var medicalControlChild = new MedicalControlTracking() {
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

            var medicalControlParentWithoutChild = new MedicalControl
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

            var medicalControlParentWithoutChildResponse = new MedicalControlResponse
            {
                Id = medicalControlParentWithoutChild.Id,
                IdEmployee = medicalControlParentWithoutChild.Employee.Id,
                IdControlType = medicalControlParentWithoutChild.ControlType.Id,
                IdAction = medicalControlParentWithoutChild.Action.Id,
                Date = medicalControlParentWithoutChild.Date,
                IdMedicalService = medicalControlParentWithoutChild.MedicalService.Id,
                IdOccupationalDoctor = medicalControlParentWithoutChild.OccupationalDoctor.Id,
                IdAbsence = medicalControlParentWithoutChild.Absence.Id,
                Diagnosis = medicalControlParentWithoutChild.Diagnosis,
                BreakTime = medicalControlParentWithoutChild.BreakTime,
                PrivateDoctorEnrollment = medicalControlParentWithoutChild.PrivateDoctorEnrollment,
                PrivateDoctorName = medicalControlParentWithoutChild.PrivateDoctorName,
                IdFile = medicalControlParentWithoutChild.IdFile,
                TestDate = medicalControlParentWithoutChild.TestDate,
                TestResult = medicalControlParentWithoutChild.TestResult,
                Tracking = new List<MedicalControlTrackingResponse>()
            };

            var medicalControlChildResponse = new MedicalControlTrackingResponse {
                Id = medicalControlChild.Id,
                IdEmployee = medicalControlChild.Employee.Id,
                IdControlType = medicalControlChild.ControlType.Id,
                IdAction = medicalControlChild.Action.Id,
                Date = medicalControlChild.Date,
                IdMedicalService = medicalControlChild.MedicalService.Id,
                IdOccupationalDoctor = medicalControlChild.OccupationalDoctor.Id,
                IdAbsence = medicalControlChild.Absence.Id,
                Diagnosis = medicalControlChild.Diagnosis,
                BreakTime = medicalControlChild.BreakTime,
                PrivateDoctorEnrollment = medicalControlChild.PrivateDoctorEnrollment,
                PrivateDoctorName = medicalControlChild.PrivateDoctorName,
                IdFile = medicalControlChild.IdFile,
                TestDate = medicalControlChild.TestDate,
                TestResult = medicalControlChild.TestResult,
                IdParent = medicalControlChild.IdParent
            };
            
            var medicalControlParentWithChild = new MedicalControl
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
                    medicalControlChild
                }
            };

            var medicalControlParentWithChildResponse = new MedicalControlResponse
            {
                Id = medicalControlParentWithoutChild.Id,
                IdEmployee = medicalControlParentWithoutChild.Employee.Id,
                IdControlType = medicalControlParentWithoutChild.ControlType.Id,
                IdAction = medicalControlParentWithoutChild.Action.Id,
                Date = medicalControlParentWithoutChild.Date,
                IdMedicalService = medicalControlParentWithoutChild.MedicalService.Id,
                IdOccupationalDoctor = medicalControlParentWithoutChild.OccupationalDoctor.Id,
                IdAbsence = medicalControlParentWithoutChild.Absence.Id,
                Diagnosis = medicalControlParentWithoutChild.Diagnosis,
                BreakTime = medicalControlParentWithoutChild.BreakTime,
                PrivateDoctorEnrollment = medicalControlParentWithoutChild.PrivateDoctorEnrollment,
                PrivateDoctorName = medicalControlParentWithoutChild.PrivateDoctorName,
                IdFile = medicalControlParentWithoutChild.IdFile,
                TestDate = medicalControlParentWithoutChild.TestDate,
                TestResult = medicalControlParentWithoutChild.TestResult,
                Tracking = new List<MedicalControlTrackingResponse>() { medicalControlChildResponse}
            };

            var mockMedicalControlManager = _autoMoqer.GetMock<IMedicalControlManager>();
            mockMedicalControlManager.Setup(x => x.Get(idMedicalControlParent, false))
                .Returns(medicalControlParentWithoutChild);
           
            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<MedicalControlResponse>(medicalControlParentWithoutChild))
                .Returns(medicalControlParentWithoutChildResponse);

            mockMapper.Setup(x => x.Map<MedicalControlResponse>(medicalControlParentWithChild))
               .Returns(medicalControlParentWithChildResponse);

            //ACT
            var result = _medicalControlController.Get(idMedicalControlParent, false);
            var model = (OkObjectResult)result.Result;
            var value = (MedicalControlResponse)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            Assert.Equal(value.Id, medicalControlParentWithoutChild.Id);
            Assert.Equal(value.IdEmployee, medicalControlParentWithoutChild.Employee.Id);
            Assert.Equal(value.IdControlType, medicalControlParentWithoutChild.ControlType.Id);
            Assert.Equal(value.IdAction, medicalControlParentWithoutChild.Action.Id);
            Assert.Equal(value.Date, medicalControlParentWithoutChild.Date);
            Assert.Equal(value.IdMedicalService, medicalControlParentWithoutChild.MedicalService.Id);
            Assert.Equal(value.IdOccupationalDoctor, medicalControlParentWithoutChild.OccupationalDoctor.Id);
            Assert.Equal(value.IdAbsence, medicalControlParentWithoutChild.Absence.Id);
            Assert.Equal(value.Diagnosis, medicalControlParentWithoutChild.Diagnosis);
        }

        [Fact]
        public void GetMedicalControlWithTracking()
        {
            //ARRANGE
            int idMedicalControlParent = 1;
            int idMedicalControlChild = 2;

            var medicalControlChild = new MedicalControlTracking()
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

            var medicalControlParentWithoutChild = new MedicalControl
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

            var medicalControlParentWithoutChildResponse = new MedicalControlResponse
            {
                Id = medicalControlParentWithoutChild.Id,
                IdEmployee = medicalControlParentWithoutChild.Employee.Id,
                IdControlType = medicalControlParentWithoutChild.ControlType.Id,
                IdAction = medicalControlParentWithoutChild.Action.Id,
                Date = medicalControlParentWithoutChild.Date,
                IdMedicalService = medicalControlParentWithoutChild.MedicalService.Id,
                IdOccupationalDoctor = medicalControlParentWithoutChild.OccupationalDoctor.Id,
                IdAbsence = medicalControlParentWithoutChild.Absence.Id,
                Diagnosis = medicalControlParentWithoutChild.Diagnosis,
                BreakTime = medicalControlParentWithoutChild.BreakTime,
                PrivateDoctorEnrollment = medicalControlParentWithoutChild.PrivateDoctorEnrollment,
                PrivateDoctorName = medicalControlParentWithoutChild.PrivateDoctorName,
                IdFile = medicalControlParentWithoutChild.IdFile,
                TestDate = medicalControlParentWithoutChild.TestDate,
                TestResult = medicalControlParentWithoutChild.TestResult,
                Tracking = new List<MedicalControlTrackingResponse>()
            };

            var medicalControlChildResponse = new MedicalControlTrackingResponse
            {
                Id = medicalControlChild.Id,
                IdEmployee = medicalControlChild.Employee.Id,
                IdControlType = medicalControlChild.ControlType.Id,
                IdAction = medicalControlChild.Action.Id,
                Date = medicalControlChild.Date,
                IdMedicalService = medicalControlChild.MedicalService.Id,
                IdOccupationalDoctor = medicalControlChild.OccupationalDoctor.Id,
                IdAbsence = medicalControlChild.Absence.Id,
                Diagnosis = medicalControlChild.Diagnosis,
                BreakTime = medicalControlChild.BreakTime,
                PrivateDoctorEnrollment = medicalControlChild.PrivateDoctorEnrollment,
                PrivateDoctorName = medicalControlChild.PrivateDoctorName,
                IdFile = medicalControlChild.IdFile,
                TestDate = medicalControlChild.TestDate,
                TestResult = medicalControlChild.TestResult,
                IdParent = medicalControlChild.IdParent
            };

            var medicalControlParentWithChild = new MedicalControl
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
                    medicalControlChild
                }
            };

            var medicalControlParentWithChildResponse = new MedicalControlResponse
            {
                Id = medicalControlParentWithChild.Id,
                IdEmployee = medicalControlParentWithChild.Employee.Id,
                IdControlType = medicalControlParentWithChild.ControlType.Id,
                IdAction = medicalControlParentWithChild.Action.Id,
                Date = medicalControlParentWithChild.Date,
                IdMedicalService = medicalControlParentWithChild.MedicalService.Id,
                IdOccupationalDoctor = medicalControlParentWithChild.OccupationalDoctor.Id,
                IdAbsence = medicalControlParentWithChild.Absence.Id,
                Diagnosis = medicalControlParentWithChild.Diagnosis,
                BreakTime = medicalControlParentWithChild.BreakTime,
                PrivateDoctorEnrollment = medicalControlParentWithChild.PrivateDoctorEnrollment,
                PrivateDoctorName = medicalControlParentWithChild.PrivateDoctorName,
                IdFile = medicalControlParentWithChild.IdFile,
                TestDate = medicalControlParentWithChild.TestDate,
                TestResult = medicalControlParentWithChild.TestResult,
                Tracking = new List<MedicalControlTrackingResponse>() { medicalControlChildResponse }
            };

            var mockMedicalControlManager = _autoMoqer.GetMock<IMedicalControlManager>();
            mockMedicalControlManager.Setup(x => x.Get(idMedicalControlParent, true))
                .Returns(medicalControlParentWithChild);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<MedicalControlResponse>(medicalControlParentWithChild))
               .Returns(medicalControlParentWithChildResponse);

            //ACT
            var result = _medicalControlController.Get(idMedicalControlParent, true);
            var model = (OkObjectResult)result.Result;
            var value = (MedicalControlResponse)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            Assert.Equal(value.Id, medicalControlParentWithChild.Id);
            Assert.Equal(value.IdEmployee, medicalControlParentWithChild.Employee.Id);
            Assert.Equal(value.IdControlType, medicalControlParentWithChild.ControlType.Id);
            Assert.Equal(value.IdAction, medicalControlParentWithChild.Action.Id);
            Assert.Equal(value.Date, medicalControlParentWithChild.Date);
            Assert.Equal(value.IdMedicalService, medicalControlParentWithChild.MedicalService.Id);
            Assert.Equal(value.IdOccupationalDoctor, medicalControlParentWithChild.OccupationalDoctor.Id);
            Assert.Equal(value.IdAbsence, medicalControlParentWithChild.Absence.Id);
            Assert.Equal(value.Diagnosis, medicalControlParentWithChild.Diagnosis);
        }

        [Fact]
        public void Post()
        {
            //ARRANGE
            var medicalControlRequest = new MedicalControlRequest
            {
                Id = 0,
                IdEmployee = 193,
                IdControlType = 1,
                IdAction = 1,
                Date = DateTime.Now,
                IdMedicalService = 1,
                IdOccupationalDoctor = 24,
                IdAbsence = 250,
                Diagnosis = "Lumbalgia",
                BreakTime = 5,
                PrivateDoctorEnrollment = "",
                PrivateDoctorName = "",
                IdFile = 123,
                TestDate = DateTime.Now,
                TestResult = true
            };

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

            var mockMedicalControlManager = _autoMoqer.GetMock<IMedicalControlManager>();
            mockMedicalControlManager.Setup(x => x.Create(medicalControl))
                .Returns(idMedicalControl);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<MedicalControlTracking>(medicalControlRequest))
                .Returns(medicalControl);

            //ACT
            var result = _medicalControlController.Post(medicalControlRequest);
            var model = (OkObjectResult)result.Result;
            var response = (int)model.Value;

            //ASSERT
            Assert.Equal(model.StatusCode, (int)HttpStatusCode.OK);
            mockMapper.Verify(x => x.Map<MedicalControlTracking>(medicalControlRequest));
            mockMedicalControlManager.Verify(x => x.Create(medicalControl));
            Assert.Equal(response, idMedicalControl);
        }

        [Fact]
        public void Put()
        {
            //ARRANGE
            var medicalControlRequest = new MedicalControlRequest
            {
                Id = 555,
                IdEmployee = 193,
                IdControlType = 1,
                IdAction = 1,
                Date = DateTime.Now,
                IdMedicalService = 1,
                IdOccupationalDoctor = 24,
                IdAbsence = 250,
                Diagnosis = "Lumbalgia",
                BreakTime = 5,
                PrivateDoctorEnrollment = "",
                PrivateDoctorName = "",
                IdFile = 123,
                TestDate = DateTime.Now,
                TestResult = true
            };

            var medicalControl = new MedicalControlTracking
            {
                Id = 555,
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

            var mockMedicalControlManager = _autoMoqer.GetMock<IMedicalControlManager>();
            mockMedicalControlManager.Setup(x => x.Update(medicalControl));

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<MedicalControlTracking>(medicalControlRequest))
                .Returns(medicalControl);

            //ACT                      
            var result = _medicalControlController.Put(medicalControlRequest);
            var response = (OkResult)result;

            //ASSERT
            Assert.Equal(response.StatusCode, (int)HttpStatusCode.OK);
            mockMapper.Verify(x => x.Map<MedicalControlTracking>(medicalControlRequest));
            mockMedicalControlManager.Verify(x => x.Update(medicalControl));
        }

        [Fact]
        public void Delete()
        {
            //ARRANGE
            int idMedicalControl = 3;
            var medicalControl = new MedicalControl
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


            var mockMedicalControlManager = _autoMoqer.GetMock<IMedicalControlManager>();
            mockMedicalControlManager.Setup(x => x.Get(idMedicalControl, false))
                .Returns(medicalControl);

            //ACT
            var result = _medicalControlController.Delete(idMedicalControl);           


            //ASSERT
            mockMedicalControlManager.Verify(x => x.Delete(idMedicalControl));
        }

        [Fact]
        public void RectifyAbsence()
        {
            //ARRANGE
            var changeRequest = new RectifyAbsenceRequest
            {
                OldAbsenceId = 250,
                NewAbsenceId = 373
            };

            var medicalControl = new MedicalControlTracking
            {
                Id = 555,
                Employee = new Employee { Id = 193 },
                ControlType = new MedicalControlType { Id = 1 },
                Action = new MedicalControlAction { Id = 1 },
                Date = DateTime.Now,
                MedicalService = new MedicalService { Id = 1 },
                OccupationalDoctor = new Doctor { Id = 24 },
                Absence = new Absence { Id = 250 },
                Diagnosis = "Lumbalgia",
                IdFile = 123,
                TestDate = DateTime.Now,
                TestResult = true
            };

            var mockMedicalControlManager = _autoMoqer.GetMock<IMedicalControlManager>();
            mockMedicalControlManager.Setup(x => x.RectifyAbsence(changeRequest.OldAbsenceId, changeRequest.NewAbsenceId)).Returns(medicalControl);

            //ACT                      
            var result = _medicalControlController.RectifyAbsence(changeRequest);
            var response = (OkResult)result;

            //ASSERT
            Assert.Equal(response.StatusCode, (int)HttpStatusCode.OK);
            mockMedicalControlManager.Verify(x => x.RectifyAbsence(changeRequest.OldAbsenceId, changeRequest.NewAbsenceId));
        }


        [Fact]
        public void GetMedicalControlByAbsenceId()
        {
            //ARRANGE
            int idMedicalControl = 1;
            int idAbsence= 101;

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

            var medicalControlResponse = new MedicalControlResponse
            {
                Id = medicalControl.Id,
                IdEmployee = medicalControl.Employee.Id,
                IdControlType = medicalControl.ControlType.Id,
                IdAction = medicalControl.Action.Id,
                Date = medicalControl.Date,
                IdMedicalService = medicalControl.MedicalService.Id,
                IdOccupationalDoctor = medicalControl.OccupationalDoctor.Id,
                IdAbsence = medicalControl.Absence.Id,
                Diagnosis = medicalControl.Diagnosis,
                IdFile = medicalControl.IdFile
            };

            var mockMedicalControlManager = _autoMoqer.GetMock<IMedicalControlManager>();
            mockMedicalControlManager.Setup(x => x.GetByAbsenceId(idAbsence))
                .Returns(medicalControl);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<MedicalControlResponse>(medicalControl))
                .Returns(medicalControlResponse);


            //ACT
            var result = _medicalControlController.GetByAbsenceId(idAbsence);
            var model = (OkObjectResult)result.Result;
            var value = (MedicalControlResponse)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            Assert.Equal(value.Id, medicalControl.Id);
            Assert.Equal(value.IdEmployee, medicalControl.Employee.Id);
            Assert.Equal(value.IdControlType, medicalControl.ControlType.Id);
            Assert.Equal(value.IdAction, medicalControl.Action.Id);
            Assert.Equal(value.Date, medicalControl.Date);
            Assert.Equal(value.IdMedicalService, medicalControl.MedicalService.Id);
            Assert.Equal(value.IdOccupationalDoctor, medicalControl.OccupationalDoctor.Id);
            Assert.Equal(value.IdAbsence, medicalControl.Absence.Id);
            Assert.Equal(value.Diagnosis, medicalControl.Diagnosis);
        }

        #region Patch
        [Fact]
        public void Patch()
        {
            //ARRANGE
            var medicalControlId = 1;
            var IdFile = 120;
            var operation = new JsonPatchDocument();
            var medicalControl = new MedicalControl { Id = medicalControlId };

            operation.Replace("/IdFileComplaint", IdFile);

            var mockAbsenceTypeManager = _autoMoqer.GetMock<IMedicalControlManager>();
            mockAbsenceTypeManager.Setup(x => x.Patch(medicalControlId, operation));

            //ACT
            var result = _medicalControlController.Patch(medicalControlId, operation);
            var response = (OkResult)result;

            //ASSERT
            Assert.Equal(response.StatusCode, (int)HttpStatusCode.OK);
            mockAbsenceTypeManager.Verify(x => x.Patch(medicalControlId, operation));
        }
        #endregion

    }
}
