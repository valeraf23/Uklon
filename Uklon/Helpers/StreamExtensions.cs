using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Uklon.Helpers
{
    public static class StreamExtensions
    {
        public static JObject ReadJObject(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead) throw new NotSupportedException("Can't read from this stream.");

            using var reader = new StreamReader(stream);
            var jo = (JObject) JToken.ReadFrom(new JsonTextReader(reader));

            return jo;
        }
    }
}