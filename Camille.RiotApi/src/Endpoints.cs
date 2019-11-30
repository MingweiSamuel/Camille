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
#elif USE_SYSTEXTJSON
            return System.Text.Json.JsonSerializer.Serialize(value);
#else
#error Must have one of USE_NEWTONESOFT or USE_SYSTEXTJSON set.
#endif
        }
    }
}
