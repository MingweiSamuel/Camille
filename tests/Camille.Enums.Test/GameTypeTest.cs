using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class GameTypeTest
    {
        [TestMethod]
        public void TestStrings()
        {
            foreach (var champ in (GameType[]) Enum.GetValues(typeof(GameType)))
            {
                Console.WriteLine(champ.ToString());
            }
        }
    }
}
