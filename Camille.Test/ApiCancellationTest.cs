﻿using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.ChampionV3;
using MingweiSamuel.Camille.Enums;

namespace MingweiSamuel.Camille.Test
{
    [TestClass]
    public class ApiCancellationTest : ApiTest
    {
        [TestMethod]
        public async Task SummonerCancellationTest()
        {
            var tokenSource = new CancellationTokenSource();
            var tasks = Enumerable.Range(0, 1000)
                .Select(n => Api.SummonerV4.GetBySummonerNameAsync(Region.NA1, n.ToString(), tokenSource.Token))
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
