
using System.Threading.Tasks;
using VL.Health.Infrastructure.DTO.WebApiFile;

namespace VL.Health.Infrastructure
{
    public interface IWebApiGateway
    {
        Task Delete(DeleteWebApiFileRequest request);
    }
}
