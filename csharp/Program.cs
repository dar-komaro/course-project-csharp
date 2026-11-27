using System;
using System.Collections.Generic;
using System.IO;
using static System.Math;
using SensorArray = System.Collections.Generic.List<Greenhouse.BaseSensor>;

namespace Greenhouse
{
    class Program
    {
        // Работа строго через интерфейс ICalibratable
        static void PerformCalibration(ICalibratable device, double offset)
        {
            if (!device.IsCalibrated)
            {
                device.Calibrate(offset);
            }
        }

        // Проверка типа во время выполнения (операторы is и as)
        static void ServiceSensor(BaseSensor sensor)
        {
            Console.Write($"Обслуживание: {sensor.Name} -> ");

            // Вариант 1: оператор 'is' с сопоставлением шаблонов
            if (sensor is ICalibratable calibratable)
            {
                PerformCalibration(calibratable, 1.5);
            }
            else
            {
                // Вариант 2: оператор 'as' возвращает null, если интерфейс не реализован
                var maybe = sensor as ICalibratable;
                if (maybe == null)
                {
                    Console.WriteLine("калибровка не требуется (не поддерживается).");
                }
            }
        }

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var tariff = new Tariff("Тариф1", 1.25);

            Console.WriteLine("\n Интерфейс ICalibratable и проверка типа (is / as)");
            BaseSensor[] sensors =
            {
                new TemperatureSensor("Датчик Температуры Т-01", tariff, 23.5),
                new HumiditySensor("Датчик Влажности В-01", tariff, 60.0),
                new LightSensor("Датчик Освещенности О-01", tariff, 4500.0),
                new FertilizerSensor("Датчик Удобрений У-01", tariff, 50.0)
            };

            foreach (var sensor in sensors)
            {
                ServiceSensor(sensor);
            }

            Console.WriteLine("\nРаннее (new) и позднее (override) связывание");
            BaseSensor baseRef = new TemperatureSensor("Датчик Т-99", tariff, 25.0);
            // Вызовется BaseSensor.GetDeviceCategory(), так как метод скрыт через new (раннее связывание по типу ссылки)
            Console.WriteLine($"Раннее связывание (GetDeviceCategory): {baseRef.GetDeviceCategory()}");
            // Вызовется TemperatureSensor.PrintInfo(), так как метод virtual/override (позднее связывание по типу объекта)
            Console.WriteLine($"Позднее связывание (PrintInfo):        {baseRef.PrintInfo()}");

            Console.WriteLine("\nСтандартный интерфейс IComparable<BaseSensor> (сортировка)");
            Array.Sort(sensors); // Сортирует по алфавиту благодаря CompareTo в BaseSensor
            Console.WriteLine("Датчики после сортировки по названию:");
            foreach (var sensor in sensors)
            {
                Console.WriteLine($"  * {sensor.Name}");
            }

            Console.WriteLine("\nКонфигурирование объекта поведения");
            var smartSensor = new TemperatureSensor(" А1", tariff, 24.0);
            smartSensor.TakeMeasurement(); // Стандартный режим (-5%)
            smartSensor.Strategy = new EcoMode(); // Динамическая смена на Эко (-1%)
            smartSensor.TakeMeasurement();

            Console.WriteLine("\n Исключения (try / catch / finally) и ресурс (using)");
            var lightSensor = new LightSensor("Свет", tariff, 5000.0);

            // Использование 'using' гарантирует закрытие файла StreamWriter при любом исходе (аналог try-with-resources)
            using (var logWriter = new StreamWriter("greenhouse_log.txt", append: false))
            {
                try
                {
                    Console.WriteLine("Запускаем серию замеров до разряда батареи...");
                    for (int i = 0; i < 25; i++)
                    {
                        lightSensor.TakeMeasurement();
                        logWriter.WriteLine($"Успешный замер #{i + 1}: заряд {lightSensor.Battery}%");
                    }
                }
                catch (LowBatteryException e) // Частный тип — первым
                {
                    Console.WriteLine($"[Перехвачено LowBatteryException] {e.Message}");
                    logWriter.WriteLine($"ОШИБКА: {e.Message}");
                    Console.WriteLine(" -> Действие системы: отправляем датчик на подзарядку!");
                    lightSensor.Recharge();
                    lightSensor.TakeMeasurement();
                }
                catch (Exception e) // Общий тип — последним
                {
                    Console.WriteLine($"[Общая ошибка] {e.Message}");
                }
                finally
                {
                    // Выполняется всегда, независимо от того, было исключение или нет
                    Console.WriteLine("[finally] Сеанс мониторинга датчика завершён.");
                }
            }

            // Проверка ошибки калибровки
            try
            {
                Console.WriteLine("\nПробуем откалибровать с недопустимой поправкой 500.0...");
                lightSensor.Calibrate(500.0);
            }
            catch (InvalidCalibrationException e)
            {
                Console.WriteLine($"[Перехвачено InvalidCalibrationException] {e.Message}");
            }
            catch (GreenhouseException e)
            {
                Console.WriteLine($"[Перехвачено GreenhouseException] {e.Message}");
            }
        }
    }
}