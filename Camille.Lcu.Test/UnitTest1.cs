using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Camille.Lcu.Test
{
    [TestClass]
    //[Ignore("Required LCU to be running.")]
    public class UnitTest1
    {
        /*[TestMethod]
        public async Task TestWamp()
        {
            var lockfile = Lockfile.GetFromProcess();
            var uri = new UriBuilder("wss", "127.0.0.1", lockfile.Port).Uri;

            var client = new ClientWebSocket();
            client.Options.AddSubProtocol("wamp");
            client.Options.Credentials = new NetworkCredential("riot", lockfile.Password);
            client.Options.RemoteCertificateValidationCallback = new LcuConfig().CertificateValidationCallback;

            await client.ConnectAsync(uri, CancellationToken.None);

            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes("[5, \"OnJsonApiEvent\"]"));
            await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

            var bufferArr = new byte[4096];
            var buffer2 = new ArraySegment<byte>(bufferArr);
            var result = await client.ReceiveAsync(buffer2, CancellationToken.None);

            Console.WriteLine(result.Count);
            Console.WriteLine(Encoding.UTF8.GetString(bufferArr, 0, result.Count));
        }*/

        [TestMethod]
        public async Task TestMethod1()
        {
            var lockfile = Lockfile.GetFromProcess();
            var lcu = new Lcu(lockfile);

            await lcu.Connect();

            var obj = await lcu.LolLogin().PostSessionV1Async(new LolLogin.UsernameAndPassword
            {
                Username = "0x5A79",
                Password = "wrong password",
            });
            //var obj = await lcu.Send<object>(new HttpRequestMessage(HttpMethod.Get, "/swagger/v2/openapi.json"));

            Console.WriteLine(obj);
        }
    }
}
