using System.Net.Http;

namespace MingweiSamuel.Camille
{
    public class Class1
    {
        void test()
        {
            using (var httpClient = new HttpClient())
            {
                var json = httpClient.GetStringAsync("url").Result;

                // Now parse with JSON.Net
            }
        }
    }
}
