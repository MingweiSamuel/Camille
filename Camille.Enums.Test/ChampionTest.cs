using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class ChampionTest
    {
        [TestMethod]
        public void TestStrings()
        {
            foreach (var champ in (Champion[]) Enum.GetValues(typeof(Champion)))
            {
                Console.WriteLine(champ.ToString());
                Assert.AreEqual(champ, Enum.Parse(typeof(Champion), champ.ToString()));
            }
        }
    }
}
