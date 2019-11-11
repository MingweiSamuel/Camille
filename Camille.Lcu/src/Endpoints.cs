using System.Runtime.CompilerServices;

namespace Camille.Lcu
{
    public abstract class Endpoints
    {
        protected readonly Lcu Lcu;

        protected Endpoints(Lcu lcu)
        {
            Lcu = lcu;
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
