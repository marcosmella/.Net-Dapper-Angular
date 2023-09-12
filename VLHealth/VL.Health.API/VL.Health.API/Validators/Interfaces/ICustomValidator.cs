using System.Collections.Generic;
using VL.Health.Domain.Enums;

namespace VL.Health.API.Validators.Interfaces
{
    public interface ICustomValidator<T>
    {
        List<string> Errors { get; }
        bool IsValid(T t, ActionType actionType);
    }
}
