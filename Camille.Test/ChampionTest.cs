using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;

namespace MingweiSamuel.Camille.Test
{
    [TestClass]
    public class ChampionTest
    {
        [TestMethod]
        public void TestStrings()
        {
            foreach (Champion champ in Enum.GetValues(typeof(Champion)))
            {
                Console.WriteLine(champ.Name());
                Assert.AreEqual(champ, ChampionUtils.Parse(champ.Name()));
                Assert.AreEqual(champ, ChampionUtils.Parse(champ.Identifier()));
            }
        }
    }
}
