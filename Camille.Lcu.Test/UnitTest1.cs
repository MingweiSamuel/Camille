using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Camille.Lcu.Test
{
    [TestClass]
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

            //var content = await lcu.SendTest();
            var content = await lcu.GetLolSummonerV1CurrentSummoner();
            var content2 = await lcu.GetLolLoginV1Session();

            Console.WriteLine("Content: " + content + "\n" + content2);
        }
    }
}
