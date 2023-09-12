
using VL.Health.Domain.Enums;

namespace VL.Health.Infrastructure.DTO.WebApiFile
{
    public class DeleteWebApiFileRequest
    {
        public EntityType EntityTypeId { get; set; }
        public int EntityId { get; set; }
        public int FileId { get; set; }
        public string Type { get; set; }
    }
}
