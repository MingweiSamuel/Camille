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
        [TestMethod]
        public async Task TestMethod1()
        {
            var processes = Process.GetProcessesByName("LeagueClient");
            if (processes.Length <= 0)
            {
                Console.WriteLine("NOT FOUND!");
                return;
            }

            var execPath = processes[0].MainModule.FileName;
            Console.WriteLine(execPath);

            var path = Path.GetDirectoryName(execPath);
            var lockfilePath = Path.Combine(path, "lockfile");
            var lockfile = Lockfile.Parse(lockfilePath);

            var lcu = new Lcu(lockfile);

            Console.WriteLine(lockfile.Port);

            var content = await lcu.SendRequestTest();

            Console.WriteLine("Content: " + content);
        }
    }
}
