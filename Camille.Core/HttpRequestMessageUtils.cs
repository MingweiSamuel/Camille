using System;
using System.Net.Http;

namespace Camille.Core
{
    public class HttpRequestMessageUtils
    {
        /// <summary>
        /// Creates a copy of the given message, directly copying over the Content field if it
        /// exists. Only works with ByteArrayContent (and subclasses e.g. StringContent) since
        /// ByteArrayContent is reusable after the message is sent.
        /// </summary>
        /// <param name="msg">Message to create a copy of. May be already sent.</param>
        /// <exception cref="ArgumentException">
        ///     Thrown if msg.Content is not null and not a ByteArrayContant (or subclass) instance
        /// </exception>
        /// <returns>A copy of the message.</returns>
        public static HttpRequestMessage Copy(HttpRequestMessage msg)
        {
            var copy = new HttpRequestMessage(msg.Method, msg.RequestUri);

            if (msg.Content != null)
            {
                if (!(msg.Content is ByteArrayContent))
                    throw new ArgumentException("Can only clone requests with ByteArrayContent "
                        + "(or subclass) or null content. ByteArrayContent is reusable.");
                copy.Content = msg.Content;
            }

            copy.Version = msg.Version;

            foreach (var prop in msg.Properties)
                copy.Properties.Add(prop);

            foreach (var header in msg.Headers)
                copy.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return copy;
        }
    }
}
