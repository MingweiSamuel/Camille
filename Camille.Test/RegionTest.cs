using Microsoft.VisualStudio.TestTools.UnitTesting;
using MingweiSamuel.Camille.Enums;

namespace MingweiSamuel.Camille.Test
{
    [TestClass]
    public class RegionTest
    {
        [TestMethod]
        public void TestGetLol()
        {
            var lolRegions = new Region[] {
                Region.BR, Region.EUNE, Region.EUW, Region.NA, Region.KR, Region.LAN,
                Region.LAS, Region.OCE, Region.RU, Region.TR, Region.JP
            };
            foreach (var lolRegion in lolRegions)
            {
                Assert.AreEqual(lolRegion, Region.Get(lolRegion.Key));
                Assert.AreEqual(lolRegion, Region.Get(lolRegion.Platform));
            }
        }

        [TestMethod]
        public void TestGetVal()
        {
            var valRegions = new Region[] {
                Region.VAL_AP, Region.VAL_BR, Region.VAL_EU, Region.VAL_KR,
                Region.VAL_LATAM, Region.VAL_NA, Region.VAL_PBE
            };
            foreach (var valRegion in valRegions)
            {
                Assert.AreEqual(valRegion, Region.GetValorant(valRegion.Key));
                Assert.AreEqual(valRegion, Region.GetValorant(valRegion.Platform));
            }
        }
    }
}
