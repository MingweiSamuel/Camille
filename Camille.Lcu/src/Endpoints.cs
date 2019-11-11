using System.Collections.Generic;
using System.Linq;

namespace Camille.Lcu
{
    public abstract class Endpoints
    {
        protected static readonly IEnumerable<KeyValuePair<string, string>> QUERY_PARAMS_EMPTY =
            Enumerable.Empty<KeyValuePair<string, string>>();

        protected readonly Lcu Lcu;

        protected Endpoints(Lcu lcu)
        {
            Lcu = lcu;
        }
    }
}
