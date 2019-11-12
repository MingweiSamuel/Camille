using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Camille.Lcu.Test
{
    [TestClass]
    [Ignore("Required LCU to be running.")]
    public class UnitTest1
    {
        /*var processes = Process.GetProcessesByName("LeagueClient");
        if (processes.Length <= 0)
        {
            Console.WriteLine("NOT FOUND!");
            return;
        }

        var execPath = processes[0].MainModule.FileName;
        Console.WriteLine(execPath);

                var path = Path.GetDirectoryName(execPath);
        var lockfilePath = Path.Combine(path, "lockfile");
        var lockfile = Lockfile.Parse(lockfilePath);*/

        [TestMethod]
        public async Task TestMethod1()
        {
            var lcu = new Lcu();

            var session = await lcu.LolLogin().PostLolLoginV1SessionAsync(new LolLogin.UsernameAndPassword
            {
                Username = "0x5A79",
                Password = "wrong password",
            });

            Console.WriteLine(session);
        }
    }
}
