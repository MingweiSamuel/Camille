using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;

namespace Camille.RiotApi.Test
{
    [TestClass]
    public class ApiCancellationTest : ApiTest
    {
        [TestMethod]
        [Ignore("Eats rate limit.")]
        public async Task SummonerCancellationTest()
        {
            var tokenSource = new CancellationTokenSource();
            var tasks = Enumerable.Range(0, 1000)
                .Select(n => Api.SummonerV4().GetBySummonerNameAsync(PlatformRoute.NA1, n.ToString(), tokenSource.Token))
                .ToList();
            tokenSource.CancelAfter(1000);
            for (var n = 0; n < tasks.Count; n++)
            {
                var task = tasks[n];
                try
                {
                    var summoner = await task;
                    if (summoner == null)
                        Console.WriteLine($"Summoner {n} is null.");
                    else
                        Assert.AreEqual(n.ToString(), Regex.Replace(summoner.Name, @"\D", ""));
                }
                catch (OperationCanceledException e) // And TaskCanceledException.
                {
                    Console.WriteLine($"Summoner {n} cancelled: {e.GetType().Name}.");
                }
            }
        }
    }
}
