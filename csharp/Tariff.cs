namespace Greenhouse
{
    public class Tariff
    {
        private double _priceUnit;

        public string Name { get; init; }

        public double PriceUnit
        {
            get => _priceUnit;
            private set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Отрицательная цена не допустима!");
                }
                _priceUnit = value;
            }
        }

        public bool UpdatePrice(double newPrice)
        {
            if (newPrice < 0.0)
            {
                return false;
            }

            PriceUnit = newPrice;
            return true;
        }

        public Tariff(string name, double priceUnit)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название тарифа не может быть пустым");
            Name = name;
            PriceUnit = priceUnit;
            Console.WriteLine($"[+Sensor] Тариф \"{Name}\" создан.");
        }

        public override string ToString() => $"Тариф: {Name}, Стоимость кВт*ч: {PriceUnit} руб.";

        public double CalculateCost(int cnt) => cnt <= 0 ? 0.0 : _priceUnit * cnt;
    }
}