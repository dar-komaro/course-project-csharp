using System;

namespace Greenhouse
{
    public interface IMeasurementStrategy
    {
        int BatteryDrain { get; }
        string ModeName { get; }
        double ComputeValue(double targetValue);
    }

    public class StandardMode : IMeasurementStrategy
    {
        public int BatteryDrain => 5;
        public string ModeName => "Стандартный";

        public double ComputeValue(double targetValue)
        {
            return targetValue + (Random.Shared.Next(5) - 2) * 0.5;
        }
    }

    public class EcoMode : IMeasurementStrategy
    {
        public int BatteryDrain => 1;
        public string ModeName => "Эко (энергосбережение)";

        public double ComputeValue(double targetValue)
        {
            return targetValue + (Random.Shared.Next(9) - 4) * 1.0;
        }
    }
}
