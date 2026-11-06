using System;
using System.Collections.Generic;
using System.IO;
using static System.Math;
using SensorArray = System.Collections.Generic.List<Greenhouse.BaseSensor>;

namespace Greenhouse
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var tariff = new Tariff("Тариф1", 1.25);
            Console.WriteLine();

            // Массив датчиков разных наследников — работа через базовый тип
            Console.WriteLine("\nМассив датчиков \n");
            SensorArray sensors = new()
            {
                new TemperatureSensor("T-01", tariff, 23.5),
                new HumiditySensor("H-01", tariff, 60.0),
                new LightSensor("L-01", tariff, 4500.0),
                new FertilizerSensor("F-01", tariff, 50.0),
            };
            foreach (var s in sensors)
            {
                s.TakeMeasurement();
                Console.WriteLine($"  {s.PrintInfo()}");
            }

            // Поверхностная и глубокая копия
            Console.WriteLine("\nКопирование\n");
            var original = new HumiditySensor("H-copy", tariff, 60.0);
            var shallow = original.ShallowCopy();
            var deep = original.DeepCopy();

            Console.WriteLine($"shallow: общая ссылка = {ReferenceEquals(original.Tariff, shallow.Tariff)}");
            shallow.Tariff.UpdatePrice(99.9);
            Console.WriteLine($"  оригинал = {original.Tariff.PriceUnit} (изменился)\n");

            Console.WriteLine($"deep: общая ссылка = {ReferenceEquals(original.Tariff, deep.Tariff)}");
            deep.Tariff.UpdatePrice(5.0);
            Console.WriteLine($"  оригинал = {original.Tariff.PriceUnit} (не изменился)\n");

            // Equals / GetHashCode
            Console.WriteLine("=== 3. Equals / GetHashCode ===");
            var t1 = new TemperatureSensor("T-dup", tariff, 23.5);
            var t2 = new TemperatureSensor("T-dup", tariff, 45.0);
            Console.WriteLine($"Equals: {t1.Equals(t2)} | HashCode: {t1.GetHashCode() == t2.GetHashCode()}");
            Console.WriteLine($"HashSet.Count = {new HashSet<TemperatureSensor> { t1, t2 }.Count} (дубль)\n");

            // this
            Console.WriteLine("\n this \n");
            new TemperatureSensor("Датчик-привязка", tariff, 25.0).RegisterInTariff(tariff);
            Console.WriteLine();

            // Статические члены
            Console.WriteLine("\nСтатика \n");
            Console.WriteLine($"NextInventoryNumber() = {BaseSensor.NextInventoryNumber()}\n");

            // Теплица
            Console.WriteLine("\nТеплица \n");
            var greenhouse = new Greenhouse("Теплица1", 20.0, tariff);
            greenhouse.PrintStatus();
            greenhouse.FullInspection();
            greenhouse.PrintStatus();
            Console.WriteLine($"\n[0]: {greenhouse[0].PrintInfo()}");
            Console.WriteLine($"ToString: {greenhouse}");

            if (greenhouse[0] is TemperatureSensor temp)
            {
                temp.TargetTemp = 26.0;
                temp.TakeMeasurement();
                Console.WriteLine($"  после смены TargetTemp: {temp.PrintInfo()}");
            }
            Console.WriteLine();

            // 8. using static System.Math
            Console.WriteLine("\nusing static System.Math\n");
            Console.WriteLine($"Clamp(150, 0, 100) = {Clamp(150, 0, 100)} | Abs(-42) = {Abs(-42)}");
        }
    }
}