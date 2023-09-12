using AutoMapper;
using AutoMoqCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using VL.Health.Interfaces.Managers;
using VL.Health.Domain.Entities;
using VL.Health.Service.Controllers;
using VL.Health.Service.DTO.Doctor.Response;
using Xunit;
using System;
using VL.Health.Service.DTO.Doctor.Request;

namespace VL.Health.Service.Tests.Controllers
{
    public class DoctorControllerTests
    {
        private readonly AutoMoqer _autoMoqer;
        private readonly DoctorController _doctorController;

        public DoctorControllerTests()
        {
            _autoMoqer = new AutoMoqer();
            _doctorController = _autoMoqer.Resolve<DoctorController>();
        }        

        [Fact]
        public void Get()
        {
            //ARRANGE
            var doctors = new List<Doctor>
            {
                new Doctor
                {
                    Id = 1,
                    FirstName = "Carlos",
                    LastName = "Montero",
                    Enrollment = "2233",
                    EnrollmentExpirationDate = DateTime.Now,
                    DocumentNumber = "123456"
                },
                new Doctor
                {
                    Id = 2,
                    FirstName = "Roberto",
                    LastName = "Perez",
                    Enrollment = "1122",
                    EnrollmentExpirationDate = DateTime.Now,
                    DocumentNumber = "654321"
                }
            };

            var doctorsResponse = new List<DoctorResponse>
            {
                new DoctorResponse
                {
                    Id = doctors[0].Id,
                    FirstName = doctors[0].FirstName,
                    LastName = doctors[0].LastName,
                    Enrollment = doctors[0].Enrollment,
                    EnrollmentExpirationDate = doctors[0].EnrollmentExpirationDate,
                    DocumentNumber = doctors[0].DocumentNumber
                },
                new DoctorResponse
                {
                    Id = doctors[1].Id,
                    FirstName = doctors[1].FirstName,
                    LastName = doctors[1].LastName,
                    Enrollment = doctors[1].Enrollment,
                    EnrollmentExpirationDate = doctors[1].EnrollmentExpirationDate,
                    DocumentNumber = doctors[1].DocumentNumber
                }
            };            

            var mockDoctorManager = _autoMoqer.GetMock<IDoctorManager>();
            mockDoctorManager.Setup(x => x.Get())
                .Returns(doctors);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<List<DoctorResponse>>(doctors))
                .Returns(doctorsResponse);

            //ACT
            var result = _doctorController.Get();
            var model = (OkObjectResult)result.Result;
            var value = (List<DoctorResponse>)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            for (var i = 0; i < doctors.Count; i++)
            {
                Assert.Equal(value[i].Id, doctorsResponse[i].Id);
                Assert.Equal(value[i].FirstName, doctorsResponse[i].FirstName);
                Assert.Equal(value[i].LastName, doctorsResponse[i].LastName);
                Assert.Equal(value[i].Enrollment, doctorsResponse[i].Enrollment);
                Assert.Equal(value[i].DocumentNumber, doctorsResponse[i].DocumentNumber);
            }
        }


        [Fact]
        public void GetById()
        {
            //ARRANGE
            int idDoctor = 5;
            var doctor = new Doctor
            {
                Id = idDoctor,
                FirstName = "Carlos",
                LastName = "Montero",
                Enrollment = "2233",
                EnrollmentExpirationDate = DateTime.Now,
                DocumentNumber = "123456"
            };

            var doctorResponse = new DoctorResponse
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Enrollment = doctor.Enrollment,
                EnrollmentExpirationDate = doctor.EnrollmentExpirationDate,
                DocumentNumber = doctor.DocumentNumber
            };

            var mockDoctorManager = _autoMoqer.GetMock<IDoctorManager>();
            mockDoctorManager.Setup(x => x.GetDoctor(idDoctor))
                .Returns(doctor);

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<DoctorResponse>(doctor))
                .Returns(doctorResponse);

            //ACT
            var result = _doctorController.Get(idDoctor);
            var model = (OkObjectResult)result.Result;
            var value = (DoctorResponse)model.Value;

            //ASSERT
            Assert.Equal((int)HttpStatusCode.OK, model.StatusCode);

            Assert.Equal(value.Id, doctor.Id);
            Assert.Equal(value.FirstName, doctor.FirstName);
            Assert.Equal(value.LastName, doctor.LastName);
            Assert.Equal(value.Enrollment, doctor.Enrollment);
            Assert.Equal(value.DocumentNumber, doctor.DocumentNumber);
        }



        [Fact]
        public void Post()
        {
            //ARRANGE
            int idDoctor = 2;

            var doctorRequest = new DoctorRequest
            {
                Id = idDoctor,
                FirstName = "Roberto",
                LastName = "Perez",
                Enrollment = "1122",
                EnrollmentExpirationDate = DateTime.Now,
                DocumentNumber = "654321"
            };

            var doctor = new Doctor
            {
                Id = doctorRequest.Id,
                FirstName = doctorRequest.FirstName,
                LastName = doctorRequest.LastName,
                Enrollment = doctorRequest.Enrollment,
                EnrollmentExpirationDate = doctorRequest.EnrollmentExpirationDate,
                DocumentNumber = doctorRequest.DocumentNumber
            };

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<Doctor>(doctorRequest))
                .Returns(doctor);

            var mockDoctorManager = _autoMoqer.GetMock<IDoctorManager>();
            mockDoctorManager.Setup(x => x.Create(doctor))
                .Returns(idDoctor);

            //ACT
            var result = _doctorController.Post(doctorRequest);
            var model = (OkObjectResult)result.Result;
            var response = (int)model.Value;

            //ASSERT
            Assert.Equal(model.StatusCode, (int)HttpStatusCode.OK);
            mockMapper.Verify(x => x.Map<Doctor>(doctorRequest));
            mockDoctorManager.Verify(x => x.Create(doctor));
            Assert.Equal(response, idDoctor);
        }

        [Fact]
        public void Put()
        {
            //ARRANGE
            int idDoctor = 2;

            var doctorRequest = new DoctorRequest
            {
                Id = idDoctor,
                FirstName = "Roberto",
                LastName = "Perez",
                Enrollment = "1122",
                EnrollmentExpirationDate = DateTime.Now,
                DocumentNumber = "654321"
            };

            var doctor = new Doctor
            {
                Id = doctorRequest.Id,
                FirstName = doctorRequest.FirstName,
                LastName = doctorRequest.LastName,
                Enrollment = doctorRequest.Enrollment,
                EnrollmentExpirationDate = doctorRequest.EnrollmentExpirationDate,
                DocumentNumber = doctorRequest.DocumentNumber
            };

            var mockMapper = _autoMoqer.GetMock<IMapper>();
            mockMapper.Setup(x => x.Map<Doctor>(doctorRequest))
                .Returns(doctor);

            var mockDoctorManager = _autoMoqer.GetMock<IDoctorManager>();
            mockDoctorManager.Setup(x => x.Update(doctor));

            //ACT                      
            var result = _doctorController.Put(doctorRequest);
            var response = (OkResult)result;

            //ASSERT
            Assert.Equal(response.StatusCode, (int)HttpStatusCode.OK);
            mockMapper.Verify(x => x.Map<Doctor>(doctorRequest));
            mockDoctorManager.Verify(x => x.Update(doctor));
        }

        [Fact]
        public void Delete()
        {
            //ARRANGE
            var idDoctor = 2;

            var mockDoctorManager = _autoMoqer.GetMock<IDoctorManager>();
            mockDoctorManager.Setup(x => x.Delete(idDoctor));
            

            //ACT
            var result = _doctorController.Delete(idDoctor);
            var response = (OkResult)result;

            //ASSERT
            Assert.Equal(response.StatusCode, (int)HttpStatusCode.OK);
            mockDoctorManager.Verify(x => x.Delete(idDoctor));
        }


    }
}
