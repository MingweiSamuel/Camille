using System;
using System.Diagnostics;
using System.IO;

namespace Camille.Lcu
{
    public class Lockfile
    {
        public readonly string ProcessName;
        public readonly ulong Pid;
        public readonly ushort Port;
        public readonly string Password;
        public readonly string Protocol;

        public Lockfile(string processName, ulong pid, ushort port, string password, string protocol)
        {
            ProcessName = processName;
            Pid = pid;
            Port = port;
            Password = password;
            Protocol = protocol;
        }

        public static Lockfile GetFromProcess(string processName = "LeagueClient")
        {
            var processes = Process.GetProcessesByName(processName);
            if (1 != processes.Length)
                throw new InvalidOperationException($"{processes.Length} processes with name \"{processName}\" found, exactly 1 needed.");

            var process = processes[0];
            if (null == process.MainModule)
                throw new InvalidOperationException($"MainModule of process with name \"{processName}\" is null.");

            var path = Path.GetDirectoryName(process.MainModule.FileName);
            Debug.Assert(null != path);
            var lockfilePath = Path.Combine(path, "lockfile");
            return ParseFile(lockfilePath);
        }

        public static Lockfile ParseFile(string lockfilePath)
        {
            string text;
            using (var stream = File.Open(lockfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            var tokens = text.Split(':');

            var process = tokens[0];
            var pid = ulong.Parse(tokens[1]);
            var port = ushort.Parse(tokens[2]);
            var password = tokens[3];
            var protocol = tokens[4];

            return new Lockfile(process, pid, port, password, protocol);
        }
    }
}
