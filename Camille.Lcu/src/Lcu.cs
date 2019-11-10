using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Camille.Lcu
{
    public class Lcu : IDisposable
    {
        private readonly LcuRequester _requester;

        public Lcu() : this(new LcuConfig()) {}

        public Lcu(LcuConfig lcuConfig)
        {
            _requester = new LcuRequester(lcuConfig);
        }

        public async Task<LolSummoner.Summoner> GetLolSummonerV1CurrentSummoner()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/lol-summoner/v1/current-summoner");
            return await SendAsync<LolSummoner.Summoner>(request);
        }

        public async Task<LolLogin.LoginSession> GetLolLoginV1Session()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/lol-login/v1/session");
            return await SendAsync<LolLogin.LoginSession>(request);
        }

        /// <summary>
        /// Send a custom request to the LCU.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            return await _requester.SendAsync<T>(request);
        }

        public void Dispose()
        {
            _requester.Dispose();
        }
    }
}
