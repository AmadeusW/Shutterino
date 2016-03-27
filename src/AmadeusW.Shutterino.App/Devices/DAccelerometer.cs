using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace AmadeusW.Shutterino.App.Devices
{
    public class DAccelerometer : Device
    {
        public double Roll => IsAvailable ? _currentReading.AccelerationX : 0d;
        public double Yaw => IsAvailable ? _currentReading.AccelerationY : 0d;
        public double Pitch => IsAvailable ? _currentReading.AccelerationZ : 0d;

        public double CapturedRoll { get; private set; }
        public double CapturedPitch { get; private set; }
        public double CapturedYaw { get; private set; }

        public double DeltaRoll => Math.Abs(Pitch - CapturedPitch);
        public double DeltaPitch => Math.Abs(Pitch - CapturedPitch);
        public double DeltaYaw => Math.Abs(Pitch - CapturedPitch);

        private readonly Accelerometer _accelerometer = Accelerometer.GetDefault();
        private AccelerometerReading _currentReading;

        public static DAccelerometer Instance { get; private set; }

        public DAccelerometer() : base()
        {
            Instance = this;
        }

        //private Vector3

        public async override Task CleanUpAsync()
        {
            if (_accelerometer == null)
            {
                return;
            }
            _accelerometer.ReadingChanged -= _accelerometer_ReadingChanged;
        }

        public async override Task<bool> InitializeAsync()
        {
            if (_accelerometer == null)
            {
                IsAvailable = false;
                return false;
            }
            _accelerometer.ReportInterval = 20;
            _accelerometer.ReadingChanged += _accelerometer_ReadingChanged;
            IsAvailable = true;
            return true;
        }

        // TODO:
        /*
        void displayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            if (null != accelerometerReadingTransform)
            {
                accelerometerReadingTransform.ReadingTransform = sender.CurrentOrientation;
            }
        }
    */

        private void _accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            _currentReading = args.Reading;
        }

        public void Callibrate()
        {
            CapturedRoll = Roll;
            CapturedPitch = Pitch;
            CapturedYaw = Yaw;
        }
    }
}
