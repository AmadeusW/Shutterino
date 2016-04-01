using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;

namespace AmadeusW.Shutterino.App.Features
{
    public class AccelerometerFeature : AFeature
    {
        // Configuration
        public double Precision { get; set; }
        public double RollOffset { get; set; }
        public double PitchOffset { get; set; }
        public int RateLimiter { get; internal set; }

        // Readings
        public double Roll => IsActive ? _currentReading.AccelerationX /** Math.Sign(_currentReading.AccelerationY)*/ : 0d;
        public double Pitch => IsActive ? _currentReading.AccelerationZ : 0d;

        public double TargetRoll => CapturedRoll + RollOffset;
        public double TargetPitch => CapturedPitch + PitchOffset;

        public double CapturedRoll { get; private set; }
        public double CapturedPitch { get; private set; }

        public double DeltaRoll => Math.Abs(TargetRoll - Roll);
        public double DeltaPitch => Math.Abs(TargetPitch - Pitch);

        private readonly Accelerometer _accelerometer = Accelerometer.GetDefault();
        private AccelerometerReading _currentReading = default(AccelerometerReading);
        private readonly DisplayInformation _displayInformation = DisplayInformation.GetForCurrentView();
        private long _lastPhotoTime;

        public static AccelerometerFeature Instance { get; private set; }

        public override string ToString() => "Accelerometer";

        public AccelerometerFeature() : base()
        {
            Instance = this;
            IsAvailable = _accelerometer != null;

            Precision = (double)(_localSettings.Values["accelerometer-Precision"] ?? 0.05);
            RollOffset = (double)(_localSettings.Values["accelerometer-RollOffset"] ?? 0d);
            PitchOffset = (double)(_localSettings.Values["accelerometer-PitchOffset"] ?? 0d);
            RateLimiter = (int)(_localSettings.Values["accelerometer-RateLimiter"] ?? Convert.ToInt32(TimeSpan.TicksPerSecond * 2));
            IsActive = (bool)(_localSettings.Values["accelerometer-IsActive"] ?? false);
        }

        public async override Task DeactivateAsync()
        {
            if (!IsAvailable || !_isActuallyActive)
                return;

            if (_displayInformation != null)
            {
                //_displayInformation.OrientationChanged -= displayInformation_OrientationChanged;
                //_accelerometer.ReadingTransform = _displayInformation.CurrentOrientation;
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
                    //_displayInformation.OrientationChanged += displayInformation_OrientationChanged;
                }

                _accelerometer.ReportInterval = 10;
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
            try
            {
                _currentReading = args.Reading;
                System.Diagnostics.Debug.WriteLine($"Reading: {_currentReading.AccelerationX:f3}, {_currentReading.AccelerationY:f3}, {_currentReading.AccelerationZ:f3}");

                if (ShutterinoLogic.Instance.TakesPhotos
                    && _currentReading.Timestamp.UtcTicks > _lastPhotoTime + RateLimiter
                    && DeltaRoll < Precision
                    && DeltaPitch < Precision)
                {
                    Callibrate(); // record current reading for future reference
                    _lastPhotoTime = _currentReading.Timestamp.UtcTicks;
                    Task.Run(async () => await ShutterinoLogic.Instance.SuggestPhotoOpportunity(this));
                }
            }
            catch (Exception ex)
            {
                // LOG
                Status = ex.ToString();
            }
        }

        public void Callibrate()
        {
            CapturedRoll = Roll;
            CapturedPitch = Pitch;
        }

        public override async Task InitializeAsync()
        {
            // There is nothing to do
            return;
        }

        public override async Task CleanupAsync()
        {
            await DeactivateAsync();

            _localSettings.Values["accelerometer-Precision"] = Precision;
            _localSettings.Values["accelerometer-RollOffset"] = RollOffset;
            _localSettings.Values["accelerometer-PitchOffset"] = PitchOffset;
            _localSettings.Values["accelerometer-RateLimiter"] = RateLimiter;
            _localSettings.Values["accelerometer-IsActive"] = IsActive;
        }
    }
}
