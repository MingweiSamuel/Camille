using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Camille.Lcu
{
    public abstract class Endpoints
    {
        public static readonly HttpMethod HttpMethodPatch =
#if USE_HTTPMETHOD_PATCH_SHIM
            new HttpMethod("PATCH");
#else
            HttpMethod.Patch;
#endif


        protected readonly ILcuApi @base;

        protected Endpoints(ILcuApi @base)
        {
            this.@base = @base;
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
