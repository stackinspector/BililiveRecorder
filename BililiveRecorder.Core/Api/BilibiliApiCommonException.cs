using System;
using System.Runtime.Serialization;

namespace BililiveRecorder.Core.Api
{
    public class BilibiliApiCommonException : Exception
    {
        public BilibiliApiCommonException() { }
        public BilibiliApiCommonException(string message) : base(message) { }
        public BilibiliApiCommonException(string message, Exception innerException) : base(message, innerException) { }
        protected BilibiliApiCommonException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
