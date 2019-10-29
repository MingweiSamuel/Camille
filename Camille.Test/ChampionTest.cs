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
                Console.WriteLine(champ.ToString());
                Assert.AreEqual(champ, Enum.Parse(typeof(Champion), champ.ToString()));
            }
        }
    }
}
