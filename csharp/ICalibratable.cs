namespace Greenhouse
{
    public interface ICalibratable
    {
        void Calibrate(double offset);
        bool IsCalibrated { get; }
    }
}
