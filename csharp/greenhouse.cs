using System;

namespace Greenhouse
{
    public class Greenhouse
    {
        private readonly BaseSensor[] _sensors;
        private double _temperature;
        public string Title {  get; init; }

        public Greenhouse(string title, double startTemperature, Tariff tariff)
        {
            Title = title;
            Temperature = startTemperature;
            _sensors = new BaseSensor[]
            {
                new TemperatureSensor("Датчик Температуры T1", tariff, 24.0),
                new HumiditySensor("Датчик Влажности H1", tariff, 65.0),
                new LightSensor("Датчик Освещенности L1", tariff, 5000.0),
                new FertilizerSensor("Датчик Удобрений F1", tariff, 60.0)
            };

            Console.WriteLine($"[+Greenhouse] Теплица \"{Title}\" создана.");
        }

        public double Temperature
        {
            get => _temperature;
            private set
            {
                if (value < -20.0 || value > 50.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Некорректная температура для теплицы!");
                }
                _temperature = value;
            }
        }

        public void FullInspection()
        {
            Console.WriteLine($"\n--- Запуск опроса 4-х датчиков теплицы \"{Title}\" ---");

            foreach (var sensor in _sensors)
            {
                sensor.TakeMeasurement();
            }
        }

        public override string ToString() => $"Теплица: {Title} | Температура: {Temperature}°C";

        public void PrintStatus()
        {
            Console.WriteLine($"Теплица: {Title}");
            Console.WriteLine("Состояние установленных приборов:");
            foreach (var sensor in _sensors)
            {
                Console.WriteLine($"  -> {sensor.PrintInfo()}");
            }
        }

        public BaseSensor this[int index]
        {
            get
            {
                if (index < 0 || index >= _sensors.Length)
                    throw new IndexOutOfRangeException(
                        $"Датчик с индексом {index} не существует. В теплице {_sensors.Length} датчика.");
                return _sensors[index];
            }
        }
    }
}
