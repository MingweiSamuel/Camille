using Camille.Lcu.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Camille.Lcu.Test
{
    [TestClass]
    [Ignore("Required LCU to be running.")]
    public class UnitTest1
    {
        //[TestMethod]
        //public async Task TestLcuRest()
        //{

        //}

        [TestMethod]
        public async Task TestMethod1()
        {
            var lockfile = Lockfile.GetFromProcess();
            using var lcu = new LcuApi(lockfile);
            var wamp = lcu.wamp;

            wamp.OnConnect += async () =>
            {
                Console.WriteLine("Connected!");
                await wamp.Subscribe("OnJsonApiEvent_lol-login_v1_session", async (data) =>
                {
                    Console.WriteLine("Logged in: " + data);
                    await wamp.Close();
                    return HandlerResult.Success;
                });
            };
            wamp.OnDisconnect += () =>
            {
                Console.WriteLine("Disconnected!");
                return null;
            };

            wamp.Connect();

            var obj = await lcu.LolLogin().PostSessionV1Async(new LolLogin.UsernameAndPassword
            {
                Username = "0x5A79",
                Password = "wrongpassword",
            });
        }
    }
}
