using System;
using System.Collections.Generic;

namespace VL.Health.API.Helpers.Interfaces
{
    public interface IHelperResultValidator
    {
        Object ObjectResult<Object>(Func<int, Object> method, int id);
        Object ObjectResult<Object>(Func<int, int, Object> method, int firstParam, int secondParam);
        int IntegerResult<Object>(Func<Object, int> method, Object entity, bool throwException = true);
        int IntegerResult(Func<int, int> method, int id);
        int IntegerResult(Func<List<Object>, int> method, List<Object> list);
        List<Object> ListResult<Object>(Func<int, List<Object>> method, int id);
        List<Object> ListResult<Object>(Func<List<Object>> method);
        List<Object> ListResult<Object>(Func<int, int, List<Object>> method, int firstParam, int secondParam);
    }
}
