using System;
using System.Runtime.CompilerServices;

namespace Greenhouse
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Композиция и Агрегация\n");
            Tariff tar = new Tariff("Обычный", 1.25);
            Greenhouse greenhouse = new Greenhouse("Теплица 1", 22.0, tar);
            Console.WriteLine(greenhouse);

            Console.WriteLine("\nПроверка правила\n");
            Sensor d1 = greenhouse[1];
            greenhouse.FullInspection();

            // Разряжаю батарею
            for (int i = 0; i < 20; i++)
            {
                d1.TakeMeasurement();
            }

            // Попытка опроса разряженного датчика
            Console.WriteLine("\nПопытка опроса с разряженной батареей:\n");
            greenhouse.FullInspection();

            // Заряжаем датчик обратно
            d1.Recharge();

            Console.WriteLine("\nСсылки и копии\n");
            Tariff t2 = tar;
            t2.UpdatePrice(2.5); // Меняю цену
            Console.WriteLine($"t2 == tar: {t2 == tar}");

            // Структура
            Point p1 = new Point(10, 20);
            Point p2 = p1;
            p2.X = 99;
            Console.WriteLine($"Структура p1.X = {p1.X}, p2.X = {p2.X}");

            Console.WriteLine("\nIDisposable и using\n");
            using (var log = new GreenhouseLog("Журнал теплицы"))
            {
                log.Write("Выполнен плановый осмотр");
            }

            Console.WriteLine("\nСборщик мусора\n");
            WeakReference<Tariff> watcher = CreateTemporaryTariff();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Console.WriteLine($"Объект жив после сборки мусора {watcher.TryGetTarget(out _)}\n");
        }


        // Пример структуры для пункта со сравнением классов и структур
        public struct Point
        {
            public int X;
            public int Y;
            public Point(int x, int y) { X = x; Y = y; }
        }

        //освобождение ресурса
        public sealed class GreenhouseLog : IDisposable
        {
            private readonly string _title;
            public GreenhouseLog(string title)
            {
                _title = title;
                Console.WriteLine($"[+] Ресурс открыт: {_title}");
            }
            public void Write(string message) => Console.WriteLine($"Запись в журнал: {message}");
            public void Dispose() => Console.WriteLine($"[-] Ресурс освобожден: {_title}");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static WeakReference<Tariff> CreateTemporaryTariff()
        {
            Tariff temp = new Tariff("Временный тариф", 10.0);
            var watcher = new WeakReference<Tariff>(temp);
            Console.WriteLine($"Объект жив {watcher.TryGetTarget(out _)}");
            return watcher;
        }

    }
}