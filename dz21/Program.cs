using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dz21
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("--- Параллельный анализатор файлов ---");
            Console.WriteLine("Нажмите любую клавишу для отмены...");
            Console.WriteLine();

            // Список URL-адресов файлов
            List<string> urls = new List<string>
            {
                "https://www.gutenberg.org/files/11/11-0.txt",      // Alice's Adventures in Wonderland
                "https://www.gutenberg.org/files/98/98-0.txt",       // A Tale of Two Cities
                "https://www.gutenberg.org/files/1661/1661-0.txt"    // The Adventures of Sherlock Holmes
            };

            // Создаём токен отмены
            CancellationTokenSource cts = new CancellationTokenSource();

            // Запускаем проверку нажатия клавиши в отдельной задаче
            Task cancelTask = Task.Run(() =>
            {
                Console.ReadKey(true); // Ждём нажатия любой клавиши
                cts.Cancel(); // Отменяем все операции
            });

            try
            {
                // Запускаем анализ всех файлов параллельно
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < urls.Count; i++)
                {
                    string fileName = $"file{i + 1}.txt";
                    tasks.Add(AnalyzeFileAsync(urls[i], fileName, cts.Token));
                }

                // Ждём завершения всех задач
                await Task.WhenAll(tasks);

                Console.WriteLine();
                Console.WriteLine("Все операции успешно завершены.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine();
                Console.WriteLine("Операция была отменена пользователем.");
            }
        }

        // Асинхронный анализ одного файла
        static async Task AnalyzeFileAsync(string url, string fileName, CancellationToken token)
        {
            Console.WriteLine($"Запускаю анализ файла {fileName}...");

            // Шаг 1: Асинхронное чтение файла из интернета
            string content = await DownloadFileAsync(url, token);

            // Шаг 2: Долгая CPU-затратная обработка (имитация)
            await ProcessFileAsync(fileName, content, token);

            // Вывод результата
            Console.WriteLine($"- Анализ файла '{fileName}' завершен. Размер: {content.Length} символов.");
        }

        // Загрузка файла из интернета
        static async Task<string> DownloadFileAsync(string url, CancellationToken token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                return await client.GetStringAsync(url);
            }
        }

        // Имитация долгой обработки файла
        static async Task ProcessFileAsync(string fileName, string content, CancellationToken token)
        {
            int steps = 5; // Количество шагов обработки

            for (int i = 1; i <= steps; i++)
            {
                // Проверяем, была ли отмена
                token.ThrowIfCancellationRequested();

                Console.WriteLine($"Анализ {fileName}: шаг {i} из {steps}...");

                // Имитация CPU-затратной работы
                await Task.Run(() =>
                {
                    // Долгая работа (например, подсчёт чего-то)
                    int count = 0;
                    for (int j = 0; j < content.Length; j++)
                    {
                        if (char.IsLetter(content[j]))
                            count++;
                    }
                    Thread.Sleep(500); // Задержка для наглядности
                }, token);
            }
        }
    }
}
