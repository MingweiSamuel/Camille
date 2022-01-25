namespace Camille.LolGame
{
    public abstract class Endpoints
    {
        protected readonly ILolGameApi @base;

        protected Endpoints(ILolGameApi @base)
        {
            this.@base = @base;
        }
    }
}
