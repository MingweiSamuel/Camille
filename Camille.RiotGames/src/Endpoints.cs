namespace Camille.RiotGames
{
    public abstract class Endpoints
    {
        protected readonly IRiotGamesApi @base;

        protected Endpoints(IRiotGamesApi @base)
        {
            this.@base = @base;
        }
    }
}
