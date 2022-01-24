using System.Net.Http;

namespace Camille.Lcu
{
    public abstract class Endpoints
    {
        protected readonly ILcuApi @base;

        protected Endpoints(ILcuApi @base)
        {
            this.@base = @base;
        }
    }
}
