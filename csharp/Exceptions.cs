using System;

namespace Greenhouse
{
    // Базовый класс исключений 
    public class GreenhouseException : Exception
    {
        public GreenhouseException(string message) : base(message) { }
    }

    // Недостаточно заряда батареи
    public class LowBatteryException : GreenhouseException
    {
        public LowBatteryException(string sensorName, int currentBattery)
            : base($"Недостаточно заряда на датчике \"{sensorName}\" (осталось {currentBattery}%)") { }
    }

    //Некорректная поправка калибровки
    public class InvalidCalibrationException : GreenhouseException
    {
        public InvalidCalibrationException(string sensorName, double offset)
            : base($"Недопустимое смещение калибровки ({offset}) для датчика \"{sensorName}\"") { }
    }
}
