using System.Net.Http;

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
    }
}
