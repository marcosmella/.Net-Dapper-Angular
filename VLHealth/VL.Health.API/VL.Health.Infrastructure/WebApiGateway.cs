using Microsoft.Extensions.Configuration;
using VL.Health.Infrastructure.DTO.WebApiFile;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using VL.Libraries.Client.Exceptions;
using System;
using VL.Libraries.Client.Gateway;

namespace VL.Health.Infrastructure
{
    public class WebApiGateway : IWebApiGateway
    {
        private readonly IServiceGateway _srvGateway;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _url;
        private readonly string _entityName = "webapi";

        public WebApiGateway(IServiceGateway serviceGateway,
                            IHttpClientFactory httpClientFactory,
                            IConfiguration configuration)
        {
            _srvGateway = serviceGateway;
            _httpClientFactory = httpClientFactory;
            _url = configuration["WebApiBusinessServiceUrl"] + "fileManagement/files";
        }
        public Task Delete(DeleteWebApiFileRequest request)
        {
            return DeleteFile(_url, request, _entityName);
        }

        private async Task DeleteFile<TIn>(string url, TIn content, string entity)
        {
            using (var client = _httpClientFactory.CreateClient("AuthorizedClient"))
            {
                client.DefaultRequestHeaders.Accept.Add(
                 new MediaTypeWithQualityHeaderValue("application/json")
                 );
                var serialized = SerializeObject(content);
                HttpRequestMessage message = new HttpRequestMessage
                {
                    Content = serialized,
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(url)
                };
                using (HttpResponseMessage response = await client.SendAsync(message))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        await ThrowErrorResponse(entity, response);
                    }
                }
            }
        }

        private async Task ThrowErrorResponse(string entity, HttpResponseMessage response)
        {
            var contentError = await response.Content.ReadAsStringAsync();
            var customErrorResponse = JsonConvert.DeserializeObject<CustomErrorResponse>(contentError);
            customErrorResponse.Entity = entity;
            if (response.Content != null)
            {
                response.Content.Dispose();
            }
            throw new SimpleHttpResponseException(response.StatusCode, customErrorResponse, contentError);
        }

        private StringContent SerializeObject<TIn>(TIn content)
        {
            return new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
        }
    }
}