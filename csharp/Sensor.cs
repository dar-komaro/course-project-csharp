using System;

namespace Greenhouse
{
    public class Sensor
    {
        private int _battery;
        private Tariff _tariff;

        public string Name { get; init; }

        public int Battery
        {
            get => _battery;
            private set => _battery = Math.Clamp(value, 0, 100);
        }

        public Tariff Tariff
        {
            get => _tariff;
            set => _tariff = value ?? throw new ArgumentNullException(nameof(value), "Тариф не может быть null");
        }

        public Sensor(string name, Tariff tariff)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название датчика не может быть пустым");

            Name = name;
            Battery = 100;
            Tariff = tariff;
            Console.WriteLine($"[+Sensor] Датчик \"{Name}\" создан.");
        }

        public bool TakeMeasurement()
        {
            if (Battery <= 0)
            {
                Console.WriteLine($"[Ошибка {Name}] Батарея разряжена (0%)!");
                return false;
            }

            Battery -= 5;
            Console.WriteLine($"Замер выполнен ({Name}). Остаток заряда: {Battery}%");
            double cost = Tariff.CalculateCost(1);
            Console.WriteLine($"Стоимость замера по тарифу \"{Tariff.Name}\": {cost} руб.");

            return true;
        }

        public void Recharge()
        {
            Battery = 100;
            Console.WriteLine($"Датчик {Name} заряжен до 100% \n");
        }


        public override string ToString() => $"Датчик: {Name} | Батарея: {Battery}% | Тариф: {Tariff.Name}";
    }
}