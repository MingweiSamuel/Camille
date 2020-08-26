using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Camille.Enums;

namespace Camille.RiotGames
{
    public interface IRiotGamesApi
    {
        public Task<T> Send<T>(string route, string methodId, HttpRequestMessage request,
            CancellationToken? token = null, bool ignoreAppRateLimits = false);

        public Task Send(string route, string methodId, HttpRequestMessage request,
            CancellationToken? token = null, bool ignoreAppRateLimits = false);
    }
}
