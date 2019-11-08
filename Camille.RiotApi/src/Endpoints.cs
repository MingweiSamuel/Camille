using System.Collections.Generic;
using System.Linq;

namespace Camille.RiotApi
{
    public abstract class Endpoints
    {
        protected static readonly IEnumerable<KeyValuePair<string, string>> QUERY_PARAMS_EMPTY =
            Enumerable.Empty<KeyValuePair<string, string>>();

        protected readonly RiotApi RiotApi;

        protected Endpoints(RiotApi riotApi)
        {
            RiotApi = riotApi;
        }
    }
}
