using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Camille.Core.Test
{
    [TestClass]
    public class HttpRequestMessageUtilsTest
    {
        [TestMethod]
        public async Task TestCopyAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri($"https://httpbin.org");
            client.DefaultRequestHeaders.Add("X-Api-Key", "apikey");

            using var req1 = new HttpRequestMessage(HttpMethod.Post, "/post");
            req1.Content = new StringContent("Hello World!", Encoding.UTF8);

            using (var res1 = await client.SendAsync(req1))
            {
                res1.EnsureSuccessStatusCode();
                Console.WriteLine(await res1.Content.ReadAsStringAsync());
            }

            using var req2 = HttpRequestMessageUtils.Copy(req1);
            using var res2 = await client.SendAsync(req2);

            res2.EnsureSuccessStatusCode();
            Console.WriteLine(await res2.Content.ReadAsStringAsync());
        }
    }
}
