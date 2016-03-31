using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;

namespace AmadeusW.Shutterino.App.Devices
{
    public class DAccelerometer : Device
    {
        // Configuration
        public double Precision { get; set; } = LOW_PRECISION;
        public double RollOffset { get; set; } = 0.0;
        public double PitchOffset { get; set; } = 0.0;
        public int RateLimiter { get; internal set; } = (int)(TimeSpan.TicksPerSecond * 2);

        // Readings
        public double Roll => IsActive ? _currentReading.AccelerationX : 0d;
        public double Pitch => IsActive ? _currentReading.AccelerationZ : 0d;

        public double TargetRoll => CapturedRoll + RollOffset;
        public double TargetPitch => CapturedPitch + PitchOffset;

        public double CapturedRoll { get; private set; }
        public double CapturedPitch { get; private set; }

        public double DeltaRoll => Math.Abs(TargetRoll - CapturedPitch);
        public double DeltaPitch => Math.Abs(TargetPitch - CapturedPitch);

        public const double HIGH_PRECISION = 0.01;
        public const double LOW_PRECISION = 0.05;
        public const double HINT_PRECISION = 0.1;

        private readonly Accelerometer _accelerometer = Accelerometer.GetDefault();
        private AccelerometerReading _currentReading = default(AccelerometerReading);
        private readonly DisplayInformation _displayInformation = DisplayInformation.GetForCurrentView();

        public static DAccelerometer Instance { get; private set; }

        public DAccelerometer() : base()
        {
            Instance = this;
            IsAvailable = _accelerometer != null;
        }

        public async override Task DeactivateAsync()
        {
            if (!IsAvailable || !_isActuallyActive)
                return;

            if (_displayInformation != null)
            {
                _displayInformation.OrientationChanged -= displayInformation_OrientationChanged;
                _accelerometer.ReadingTransform = _displayInformation.CurrentOrientation;
            }

            _accelerometer.ReportInterval = 0;
            _accelerometer.ReadingChanged -= _accelerometer_ReadingChanged;
            _isActuallyActive = false;
        }

        public async override Task ActivateAsync()
        {
            if (!IsAvailable || _isActuallyActive)
                return;

            // Activate only if user wants to
            if (IsActive)
            {
                if (_displayInformation != null)
                {
                    _displayInformation.OrientationChanged += displayInformation_OrientationChanged;
                }

                _accelerometer.ReportInterval = 20;
                _accelerometer.ReadingChanged += _accelerometer_ReadingChanged;
                _isActuallyActive = true;
            }
        }

        void displayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            if (_accelerometer != null)
            {
                _accelerometer.ReadingTransform = sender.CurrentOrientation;
            }
        }

        private void _accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            _currentReading = args.Reading;

            if (DeltaRoll < Precision
                && DeltaPitch < Precision)
            {
                // notify of photo opportunity
            }
        }

        public void Callibrate()
        {
            CapturedRoll = Roll;
            CapturedPitch = Pitch;
        }

        public bool IsPhotoOpportunity()
        {
            return DeltaRoll < Precision
                && DeltaPitch < Precision;
        }

        public override async Task InitializeAsync()
        {
            // There is nothing to do
            return;
        }

        public override async Task CleanupAsync()
        {
            await DeactivateAsync();
        }
    }
}
