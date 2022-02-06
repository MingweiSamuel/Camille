using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Camille.Lcu.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            using var lcu = new LcuApi();

            var session = await lcu.LolLogin().GetAccountStateV1Async();
            Console.WriteLine(session);
        }
    }
}
