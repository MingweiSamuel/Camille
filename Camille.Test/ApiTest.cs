using MingweiSamuel.Camille;
using System.IO;

namespace Camille.Test
{
    public class ApiTest
    {
        protected static readonly RiotApi Api = RiotApi.NewInstance
        (
            new RiotApiConfig.Builder(File.ReadAllText("apikey.txt").Trim())
            {
                MaxConcurrentRequests = 10,
                Retries = 10
            }.Build()
        );
    }
}
