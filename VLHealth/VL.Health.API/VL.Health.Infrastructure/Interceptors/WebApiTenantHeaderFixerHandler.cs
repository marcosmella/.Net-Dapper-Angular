using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VL.Health.Infrastructure.Interceptors
{
    public class WebApiTenantHeaderFixerHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WebApiTenantHeaderFixerHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var xraet))
            {
                request.Headers.Add("X-RAET-Tenant-Id", xraet[0]);
            }

            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Ocp-Apim-Subscription-Key", out var header))
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", header[0]);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
