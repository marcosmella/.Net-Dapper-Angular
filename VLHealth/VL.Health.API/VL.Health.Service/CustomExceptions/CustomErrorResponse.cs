using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace VL.Health.Service.CustomExceptions
{
    [ExcludeFromCodeCoverage]
    public class CustomErrorResponse
    {
        public string Message { get; set; }
        public List<string> Error { get; set; }
    }
}
