using System.Runtime.CompilerServices;

namespace Camille.RiotApi
{
    public abstract class Endpoints
    {
        protected readonly IRiotApi RiotApi;

        protected Endpoints(IRiotApi riotApi)
        {
            RiotApi = riotApi;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static string JsonSerialize(object value)
        {
#if USE_NEWTONSOFT
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
#endif
#if USE_SYSTEXTJSON
            return System.Text.Json.JsonSerializer.Serialize(value);
#endif
        }
    }
}
