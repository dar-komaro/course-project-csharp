using System;

namespace Greenhouse
{
    public abstract class BaseSensor : IComparable<BaseSensor>
    {
        private static int _lastNumber = 0;
        public static int NextInventoryNumber() => ++_lastNumber;

        private int _battery;
        private Tariff _tariff;
        private IMeasurementStrategy _strategy = new StandardMode();

        public string Name { get; init; }
        public int InventoryNumber { get; init; }

        public int Battery
        {
            get => _battery;
            protected set => _battery = Math.Clamp(value, 0, 100);
        }

        public Tariff Tariff
        {
            get => _tariff;
            set => _tariff = value ?? throw new ArgumentNullException(nameof(value), "Тариф не может быть null");
        }

        public IMeasurementStrategy Strategy
        {
            get => _strategy;
            set
            {
                _strategy = value ?? throw new ArgumentNullException(nameof(value), "Стратегия не может быть null");
                Console.WriteLine($"[Конфигурация] Датчик \"{Name}\" переключен в режим: {_strategy.ModeName}");
            }
        }

        protected BaseSensor()
        {
            InventoryNumber = NextInventoryNumber();
            Name = "Универсальный";
            Battery = 100;
            Tariff = new Tariff("Стандартный (Авто)", 0.0);

            Console.WriteLine($"[+BaseSensor] Создан базовый датчик по умолчанию | Инв. №: #{InventoryNumber}");
        }

        protected BaseSensor(string name, Tariff tariff)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название датчика не может быть пустым");

            this.InventoryNumber = NextInventoryNumber();
            this.Name = name;
            this.Battery = 100;
            this.Tariff = tariff;

            Console.WriteLine($"[+BaseSensor] Датчик \"{Name}\" создан | Инв. №: #{InventoryNumber}");
        }

        public abstract bool TakeMeasurement();
        public void RegisterInTariff(Tariff tariff) => tariff.LogSensorRegistration(this);
        public void Recharge()
        {
            Battery = 100;
            Console.WriteLine($"Датчик {Name} заряжен до 100% \n");
        }

        // Невиртуальный метод раннее связывание
        public string GetDeviceCategory() => "Базовый измерительный прибор";

        public virtual string PrintInfo()
        {
            return $"Датчик: {Name} | Заряд: {Battery}% | Инв. №: #{InventoryNumber}";
        }

        public int CompareTo(BaseSensor? other)
        {
            if (other is null) return 1;
            return string.Compare(Name, other.Name, StringComparison.CurrentCulture);
        }

    }

    public class TemperatureSensor : BaseSensor
    {
        private double _currentTemp = 22.0;
        private double _targetTemp = 24.0;

        public TemperatureSensor() : base()
        {
            Console.WriteLine("[+TemperatureSensor] Создан датчик температуры по умолчанию");
        }

        public TemperatureSensor(string name, Tariff tariff, double targetTemp)
            : base(name, tariff)
        {
            TargetTemp = targetTemp;
            Console.WriteLine($"[+TemperatureSensor] Инициализирован температурный датчик \"{name}\"");
        }

        public double TargetTemp
        {
            get => _targetTemp;
            set
            {
                if (value < 0.0 || value > 50.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Целевая температура вне диапазона 0..50 °C");
                _targetTemp = value;
            }
        }

        public override bool TakeMeasurement()
        {
            int drain = Strategy.BatteryDrain;
            if (Battery < drain)
                throw new LowBatteryException(Name, Battery);

            Battery -= drain;
            _currentTemp = Strategy.ComputeValue(_targetTemp);
            Console.WriteLine($"[Замер Темп | Режим: {Strategy.ModeName}] {Name} замерил: {_currentTemp:F1} °C (остаток заряда: {Battery}%)");
            return true;
        }

        // Скрытие метода базового класса через new -> РАННЕЕ связывание
        public new string GetDeviceCategory() => "Датчик температуры";

        public override string PrintInfo() =>
            base.PrintInfo() + $" | Температура: факт = {_currentTemp:F1} °C (Цель: {_targetTemp:F1} °C)";

        public override bool Equals(object? obj) =>
            obj is TemperatureSensor other && Name == other.Name;

        public override int GetHashCode() => Name.GetHashCode();
    }

    public class HumiditySensor : BaseSensor
    {
        private double _currentHumidity = 60.0;
        private double _targetHumidity = 65.0;

        public HumiditySensor() : base()
        {
            Console.WriteLine("[+HumiditySensor] Создан датчик влажности по умолчанию");
        }

        public HumiditySensor(string name, Tariff tariff, double targetHumidity)
            : base(name, tariff)
        {
            TargetHumidity = targetHumidity;
            Console.WriteLine($"[+HumiditySensor] Инициализирован датчик влажности \"{name}\"");
        }

        public double TargetHumidity
        {
            get => _targetHumidity;
            set
            {
                if (value < 0.0 || value > 100.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Целевая влажность вне диапазона 0..100 %");
                _targetHumidity = value;
            }
        }

        public override bool TakeMeasurement()
        {
            int drain = Strategy.BatteryDrain;
            if (Battery < drain)
                throw new LowBatteryException(Name, Battery);

            Battery -= drain;
            _currentHumidity = Strategy.ComputeValue(_targetHumidity);
            Console.WriteLine($"[Замер Влажности | Режим: {Strategy.ModeName}] {Name} замерил: {_currentHumidity:F1}% (остаток заряда: {Battery}%)");
            return true;
        }

        public override string PrintInfo() =>
            base.PrintInfo() + $" | Влажность: факт = {_currentHumidity:F1}% (Цель: {_targetHumidity:F1}%)";

        //Поверхностная копия: Tariff общий (ссылка копируется как есть)
        public HumiditySensor ShallowCopy() => (HumiditySensor)MemberwiseClone();

        //Глубокая копия: Tariff заменяется собственным экземпляром
        public HumiditySensor DeepCopy()
        {
            var copy = (HumiditySensor)MemberwiseClone();
            copy.Tariff = new Tariff(Tariff.Name, Tariff.PriceUnit); // своя копия тарифа
            return copy;
        }
    }

    public sealed class LightSensor : BaseSensor, ICalibratable
    {
        private double _currentLight = 5000.0;
        private double _targetLight = 6000.0;

        public bool IsCalibrated { get; private set; } = false;

        public LightSensor() : base()
        {
            Console.WriteLine("[+LightSensor] Создан датчик освещенности по умолчанию");
        }

        public LightSensor(string name, Tariff tariff, double targetLight)
            : base(name, tariff)
        {
            TargetLight = targetLight;
            Console.WriteLine($"[+LightSensor] Инициализирован датчик освещенности \"{name}\"");
        }

        public double TargetLight
        {
            get => _targetLight;
            set
            {
                if (value < 0.0 || value > 100000.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Целевая освещённость вне диапазона 0..100000 лк");
                _targetLight = value;
            }
        }

        public override bool TakeMeasurement()
        {
            int drain = Strategy.BatteryDrain;
            if (Battery < drain)
                throw new LowBatteryException(Name, Battery);

            Battery -= drain;
            double deviation = Strategy.ComputeValue(_targetLight) - _targetLight;
            _currentLight = _targetLight + deviation * 50.0;
            Console.WriteLine($"[Замер Света | Режим: {Strategy.ModeName}] {Name} замерил: {_currentLight:F0} лк (остаток заряда: {Battery}%)");
            return true;
        }

        public void Calibrate(double offset)
        {
            if (offset < -100.0 || offset > 100.0)
                throw new InvalidCalibrationException(Name, offset);

            _currentLight += offset;
            IsCalibrated = true;
            Console.WriteLine($"[Калибровка] Датчик света \"{Name}\" откалиброван (поправка: {offset} лк).");
        }

        public new string GetDeviceCategory() => "Оптический датчик освещенности";

        public override string PrintInfo() =>
            base.PrintInfo() + $" | Свет: {_currentLight:F0} / {_targetLight:F0} лк";
    }

    public class FertilizerSensor : BaseSensor, ICalibratable
    {
        private double _currentFertilizer = 50.0;
        private double _targetFertilizer = 60.0;

        public bool IsCalibrated { get; private set; } = false;

        public FertilizerSensor() : base()
        {
            Console.WriteLine("[+FertilizerSensor] Создан датчик удобрений по умолчанию");
        }

        public FertilizerSensor(string name, Tariff tariff, double targetFertilizer)
            : base(name, tariff)
        {
            TargetFertilizer = targetFertilizer;
            Console.WriteLine($"[+FertilizerSensor] Инициализирован датчик удобрений \"{name}\"");
        }

        public double TargetFertilizer
        {
            get => _targetFertilizer;
            set
            {
                if (value < 0.0 || value > 100.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Целевой уровень удобрений вне диапазона 0..100 %");
                _targetFertilizer = value;
            }
        }

        public override bool TakeMeasurement()
        {
            int drain = Strategy.BatteryDrain;
            if (Battery < drain)
                throw new LowBatteryException(Name, Battery);
            Battery -= drain;
            _currentFertilizer = Strategy.ComputeValue(_targetFertilizer);
            Console.WriteLine($"[Замер Удобрений | Режим: {Strategy.ModeName}] {Name} замерил уровень: {_currentFertilizer:F1}% (остаток заряда: {Battery}%)");
            return true;
        }

        public void Calibrate(double offset)
        {
            if (offset < -10.0 || offset > 10.0)
                throw new InvalidCalibrationException(Name, offset);
            _currentFertilizer += offset;
            IsCalibrated = true;
            Console.WriteLine($"[Калибровка] Датчик удобрений \"{Name}\" откалиброван (поправка: {offset}%).");
        }


        public override string PrintInfo() =>
            $"Прибор удобрений \"{Name}\" | Заряд: {Battery}% | Инв. №: #{InventoryNumber} | Удобрения: {_currentFertilizer:F1} / {_targetFertilizer:F1} %";
    }



}