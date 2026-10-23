using System;

namespace Greenhouse
{
    public class Greenhouse
    {
        private readonly Sensor[] _sensors;
        private double _temperature;
        public string Title {  get; init; }

        public Greenhouse(string title, double startTemperature, Tariff tariff)
        {
            Title = title;
            Temperature = startTemperature;
            _sensors = new Sensor[]
            {
                new Sensor("D1", tariff),
                new Sensor("D2", tariff),
                new Sensor("D3", tariff),
                new Sensor("D4", tariff)
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

        public Sensor this[int index]
        {
            get
            {
                if (index < 0 || index >= 4)
                    throw new IndexOutOfRangeException("Датчик с таким индексом не существует. В теплице 4 датчика.");
                return _sensors[index];
            }
        }
    }
}
