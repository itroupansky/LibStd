using System.Collections.Generic;

namespace LibStd.Common.Exceptions
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // Used to deserialize validation errors returned from WebApis
    public class ValidationErrors
    {
        private readonly List<ValidationException.ValidationError> _errors;

        public ValidationErrors()
        {
            _errors = new List<ValidationException.ValidationError>();
        }

        public List<ValidationException.ValidationError> Errors => _errors;
    }
}
