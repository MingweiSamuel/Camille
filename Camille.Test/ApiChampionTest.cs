using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Champion;
using MingweiSamuel.Camille.Enums;

namespace Camille.Test
{
    [TestClass]
    public class ApiChampionTest : ApiTest
    {
        [TestMethod]
        public void GetAll()
        {
            CheckGetAll(Api.Champion.GetChampions(Region.NA));
        }

        [TestMethod]
        public async Task GetAllAsync()
        {
            CheckGetAll(await Api.Champion.GetChampionsAsync(Region.NA));
        }

        public static void CheckGetAll(ChampionList result)
        {
            var free = 0;
            foreach (var champ in result.Champions)
            {
                if (champ.Id == ChampionId.Zyra)
                    CheckGet(champ);
                if (champ.FreeToPlay)
                    free++;
            }
            // We're up to 14 free champions (2017/08).
            Assert.IsTrue(10 <= free && free <= 20, free.ToString());
        }

        [TestMethod]
        public void Get()
        {
            CheckGet(Api.Champion.GetChampionsById(Region.NA, ChampionId.Zyra));
        }

        [TestMethod]
        public async Task GetAsync()
        {
            CheckGet(await Api.Champion.GetChampionsByIdAsync(Region.NA, ChampionId.Zyra));
        }

        public static void CheckGet(Champion result)
        {
            Assert.AreEqual(143, result.Id);
            Assert.IsTrue(result.BotEnabled);
        }
    }
}
