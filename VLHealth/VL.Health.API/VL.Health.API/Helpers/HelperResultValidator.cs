using System;
using System.Collections.Generic;
using VL.Health.API.Helpers.Interfaces;
using VL.Health.Domain.Enums;
using VL.Health.API.Exceptions;

namespace VL.Health.API.Helpers
{
    public class HelperResultValidator: IHelperResultValidator
    {
        public Object ObjectResult<Object>(Func<int, Object> method, int id)
        {
            Object entity = method(id);
            if (entity == null)
            {
                throw new FunctionalException(ErrorType.NotFound);
            }
            return entity;
        }
        public Object ObjectResult<Object>(Func<int,int, Object> method, int firstParam, int secondParam)
        {
            Object entity = method(firstParam, secondParam);
            if (entity == null)
            {
                throw new FunctionalException(ErrorType.NotFound);
            }
            return entity;
        }

        public int IntegerResult<Object>(Func<Object, int> method, Object entity, bool throwException = true)
        {
            int affectedRows = method(entity);
            if (affectedRows > 0)
			{
                return affectedRows;
            }

            if (throwException)
            {
                throw new FunctionalException(ErrorType.NotFound);
            }

            return affectedRows; 
        }

        public int IntegerResult(Func<int, int> method, int id)
        {
            int affectedRows = method(id);
            if (affectedRows == 0)
            {
                throw new FunctionalException(ErrorType.NotFound);
            }
            return affectedRows;
        }

        public int IntegerResult(Func<List<Object>, int> method, List<Object> list)
        {
            int affectedRows = method(list);
            if (affectedRows == 0)
            {
                throw new FunctionalException(ErrorType.NotFound);
            }
            return affectedRows;
        }

        public List<Object> ListResult<Object>(Func<int, List<Object>> method, int id)
        {
            List<Object> list = method(id);
            if (list.Count == 0)
            {
                throw new FunctionalException(ErrorType.NotFound);
            }
            return list;
        }

        public List<Object> ListResult<Object>(Func<List<Object>> method)
        {
            List<Object> list = method();
            if (list.Count == 0)
            {
                throw new FunctionalException(ErrorType.NotFound);
            }
            return list;
        }

        public List<Object> ListResult<Object>(Func<int,int, List<Object>> method, int firstParam, int secondParam)
        {
            List<Object> list = method(firstParam, secondParam);
            if (list.Count == 0)
            {
                throw new FunctionalException(ErrorType.NotFound);
            }
            return list;
        }

    }
}
