using System;
using System.Runtime.Serialization;

namespace LibStd.Common.Exceptions
{
    /// <summary>
    /// Business exception to thrown when an object already exists in the database
    /// </summary>
    [Serializable]
    public class ObjectExistsException : Exception
    {
        public ObjectExistsException()
        {
        }

        public ObjectExistsException(string message) : base(message) { }

        protected ObjectExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Version of the object exists exception that automatically generates the error message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ObjectExistsException<T> : ObjectExistsException
    {
        public ObjectExistsException(object id)
            : base($"{typeof(T).Name} #{id} already exists.")
        {
        }

        protected ObjectExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
