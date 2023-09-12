using System;
using System.Collections.Generic;
using VL.Health.Domain.Enums;

namespace VL.Health.API.Exceptions
{
    public class FunctionalException : Exception
    {
        public ErrorType FunctionalError { get; }
        public List<string> Errors { get; }

        public FunctionalException(ErrorType functionalError, List<string> errors) 
            : base()
        {
            FunctionalError = functionalError;
            Errors = errors;
        }

        public FunctionalException(ErrorType functionalError, string message)
            : base()
        {
            FunctionalError = functionalError;
            Errors = new List<string> { message };
        }

        public FunctionalException(ErrorType functionalError)
            : base()
        {
            FunctionalError = functionalError;
            Errors = new List<string>();
        }
    }
}
