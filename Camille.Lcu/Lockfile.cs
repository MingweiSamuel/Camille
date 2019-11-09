using System.IO;

namespace Camille.Lcu
{
    public class Lockfile
    {
        public readonly string Process;
        public readonly ulong Pid;
        public readonly ushort Port;
        public readonly string Password;
        public readonly string Protocol;

        public Lockfile(string process, ulong pid, ushort port, string password, string protocol)
        {
            Process = process;
            Pid = pid;
            Port = port;
            Password = password;
            Protocol = protocol;
        }

        public static Lockfile Parse(string lockfilePath)
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
