using System;
using System.Runtime.Serialization;

namespace LibStd.Common.Exceptions
{
    /// <summary>
    /// Exception to throw then the current user tries to access a resource they 
    /// do not have permissions to access
    /// </summary>
    [Serializable]
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException() { }

        public AccessDeniedException(string message) : base(message) { }

        protected AccessDeniedException(SerializationInfo info,StreamingContext context) 
            : base(info, context) {}
    }

    /// <summary>
    /// Version of the AccessDeniedException that automatically generates the error message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class AccessDeniedException<T> : AccessDeniedException
    {
        public AccessDeniedException(object id)
            : base($"Access denied to {typeof(T).Name} #{id}.")
        {
        }

        protected AccessDeniedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
