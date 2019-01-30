using System;
using System.Runtime.Serialization;

namespace LibStd.Common.Exceptions
{
    /// <summary>
    /// Business exception to thrown when an object could not be retrieved from the database
    /// </summary>
    [Serializable]
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException() { }

        public ObjectNotFoundException(string message) : base(message) { }

        protected ObjectNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Version of the object not found exception that automatically generates the error message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ObjectNotFoundException<T> : ObjectNotFoundException
    {
        public ObjectNotFoundException(object id)
            : base($"{typeof(T).Name} #{id} could not be retrieved.")
        {
        }

        protected ObjectNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
