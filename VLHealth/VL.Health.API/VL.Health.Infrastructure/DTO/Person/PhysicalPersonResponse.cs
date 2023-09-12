using System;

namespace VL.Health.Infrastructure.DTO.Person
{
    public class PhysicalPersonResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Gender { get; set; }
        public int? IdMaritalStatus { get; set; }
        public int? IdNationality { get; set; }
        public int Age { get; set; }
    }
}
