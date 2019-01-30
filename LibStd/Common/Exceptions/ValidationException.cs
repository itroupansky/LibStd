using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace LibStd.Common.Exceptions
{
    /// <summary>
    /// Exception to throw when a validation error occurs
    /// </summary>
    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationException(IEnumerable<ValidationError> errors)
            : this(errors.ToArray()) { }

        private ValidationException(ValidationError[] errors)
            : base(BuildErrorMesage(errors))
        {
            Errors = errors;
        }

        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public ValidationError[] Errors { get; private set; }

        private static string BuildErrorMesage(IEnumerable<ValidationError> errors)
        {
            var arr = errors.Select(x => "\r\n -- " + x.Message).ToArray();
            return "Validation failed: " + string.Join("", arr);
        }

        public class ValidationError
        {
            public string Message { get; set; }

            public string PropertyName { get; set; }
        }
    }
}
