using System;
using System.Diagnostics;
using System.Threading;

namespace RealTimeMemoryAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Анализ использования оперативной памяти в реальном времени...");
            Console.WriteLine("Нажмите 'Q' для выхода.");

            // Запустить анализ использования памяти в отдельном потоке
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var memoryAnalysisThread = new Thread(() => MonitorMemoryUsage(cancellationToken));
            memoryAnalysisThread.Start();

            // Цикл ожидания нажатия клавиши 'Q' для выхода
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(intercept: true).Key == ConsoleKey.Q)
                {
                    cancellationTokenSource.Cancel();
                    break;
                }
            }

            // Дождаться завершения анализа памяти
            memoryAnalysisThread.Join();

            Console.WriteLine("Анализ завершен. Нажмите любую клавишу для выхода.");
            Console.ReadKey();
        }

        static void MonitorMemoryUsage(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Получить информацию о памяти
                var process = Process.GetCurrentProcess();
                var privateMemory = process.PrivateMemorySize64;
                var formattedMemory = FormatMemory(privateMemory);
                Console.WriteLine($"Использование памяти: {formattedMemory}");

                // Оптимизация памяти для приложений на C#
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                Thread.Sleep(1000); // Пауза 1 секунда перед следующим обновлением
            }
        }

        static string FormatMemory(long bytes)
        {
            const long kb = 1024;
            const long mb = kb * 1024;
            const long gb = mb * 1024;

            if (bytes >= gb)
                return $"{bytes / gb} GB";
            if (bytes >= mb)
                return $"{bytes / mb} MB";
            if (bytes >= kb)
                return $"{bytes / kb} KB";

            return $"{bytes} bytes";
        }
    }
}