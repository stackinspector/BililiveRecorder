using System;
using System.Runtime.Serialization;

namespace BililiveRecorder.Core.Api
{
    public class BilibiliApiRateLimitedException : Exception
    {
        public BilibiliApiRateLimitedException() { }
        public BilibiliApiRateLimitedException(string message) : base(message) { }
        public BilibiliApiRateLimitedException(string message, Exception innerException) : base(message, innerException) { }
        protected BilibiliApiRateLimitedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
