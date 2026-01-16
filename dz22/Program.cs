using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace dz22
{
    // Класс для представления структуры поста
    public class Post
    {
        [JsonProperty("userId")]
        public int? UserId { get; set; }  // nullable int для обработки отсутствующих значений

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }  // nullable string

        [JsonProperty("body")]
        public string Body { get; set; }
    }

    class Program
    {
        static async Task Main()
        {
            const string apiUrl = "https://jsonplaceholder.typicode.com/posts";

            try
            {
                var handler = new HttpClientHandler
                {
                    //ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    //MaxConnectionsPerServer = 1
                };

                Console.WriteLine("Загрузка списка постов...");

                // Создание HTTP-клиента
                using (HttpClient client = new HttpClient(handler))
                {
                    // Выполнение GET-запроса
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    // Чтение содержимого ответа
                    string jsonContent = await response.Content.ReadAsStringAsync();

                    // Десериализация JSON в список объектов Post
                    List<Post> posts = JsonConvert.DeserializeObject<List<Post>>(jsonContent);

                    if (posts != null && posts.Count > 0)
                    {
                        Console.WriteLine("Данные успешно загружены!\n");
                        Console.WriteLine("--- Анализ постов ---\n");

                        // Вывод первых 5 постов для демонстрации
                        int count = Math.Min(5, posts.Count);
                        for (int i = 0; i < count; i++)
                        {
                            Post post = posts[i];

                            // Использование оператора ?? для обработки null-значений
                            // ИСПРАВЛЕНИЕ: для int? используем ?? с nullable-типом
                            Console.WriteLine($"Пост №{post.Id ?? 0} (Автор ID: {post.UserId ?? 0})");
                            Console.WriteLine($"Заголовок: {post.Title ?? "Без заголовка"}");
                            Console.WriteLine($"Текст: {post.Body ?? "Без содержимого"}");
                            Console.WriteLine();
                        }

                        Console.WriteLine($"Всего загружено постов: {posts.Count}");
                    }
                    else
                    {
                        Console.WriteLine("Не удалось получить данные или список пуст.");
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Ошибка при выполнении HTTP-запроса: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"Ошибка при обработке JSON: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
            }
        }
    }
}
