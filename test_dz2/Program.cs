using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace test_dz2
{
    internal class Program
    {
        static async Task ReliableTest()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                MaxConnectionsPerServer = 1
            };

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30),
                DefaultRequestVersion = new Version(1, 1),
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
            };

            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            client.DefaultRequestHeaders.ConnectionClose = false;

            string url = "https://jsonplaceholder.typicode.com/posts";

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✓ Успешно! Размер: {json.Length} байт");
                Console.WriteLine($"Протокол: {response.Version}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка: {ex.Message}");
                Console.WriteLine($"Тип: {ex.GetType().Name}");
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
