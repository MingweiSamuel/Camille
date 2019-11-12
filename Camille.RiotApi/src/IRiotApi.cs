using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Camille.Enums;
using Camille.RiotApi.Util;

namespace Camille.RiotApi
{
    public interface IRiotApi
    {
        public T Send<T>(Region region, string methodId, bool nonRateLimited,
            HttpRequestMessage request, CancellationToken? token = null);

        public void Send(Region region, string methodId, bool nonRateLimited,
            HttpRequestMessage request, CancellationToken? token = null);

        public Task<T> SendAsync<T>(Region region, string methodId, bool nonRateLimited,
            HttpRequestMessage request, CancellationToken? token = null);

        public Task SendAsync(Region region, string methodId, bool nonRateLimited,
            HttpRequestMessage request, CancellationToken? token = null);
    }
}
