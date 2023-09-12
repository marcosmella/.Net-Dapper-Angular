using Microsoft.Extensions.Configuration;
using System;
using VL.Health.Infrastructure.DTO.Person;
using VL.Health.Infrastructure.Interfaces;
using VL.Libraries.Client.Gateway;

namespace VL.Health.Infrastructure
{
    public class PersonGateway : IPersonGateway
    {
        private readonly IServiceGateway _serviceGateway;
        private readonly Uri _url;

        public PersonGateway(IServiceGateway serviceGateway, IConfiguration configuration)
        {
            _serviceGateway = serviceGateway;
            _url = new Uri(configuration["PersonBackEndUrl"]);
        }

        public bool Exists(int idPerson)
        {
            bool exists = false;

            string resource = $"api/physical-persons/{idPerson}";

			try
			{
                var person = _serviceGateway.Get<PhysicalPersonResponse>(_url.ToString() + resource).Result;
                if (person != null)
				{
                    exists = true;
                }
            }
            catch (Exception) { }

            return exists;
        }
    }
}
