using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MingweiSamuel.Camille
{
    public abstract class Endpoints
    {
        protected readonly RiotApi RiotApi;

        protected Endpoints(RiotApi riotApi)
        {
            RiotApi = riotApi;
        }

        protected static KeyValuePair<string, string>[] MakeParams(params object[] input)
        {
            return Enumerable.Range(0, input.Length / 2)
                .SelectMany(i =>
                {
                    var k = input[2 * i].ToString();
                    var v = input[2 * i + 1];
                    if (null == v)
                        return Enumerable.Empty<KeyValuePair<string, string>>();
                    if (v is string str)
                        return new[] { new KeyValuePair<string, string>(k, str) };
                    if (v is IEnumerable enumerable)
                        return enumerable.Cast<object>()
                            .Select(w => new KeyValuePair<string, string>(k, w.ToString()));
                    if (v is bool)
                        v = v.ToString().ToLowerInvariant();
                    return new[] {new KeyValuePair<string, string>(k, v.ToString())};
                })
                .ToArray();
        }
    }
}
