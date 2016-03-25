using AmadeusW.Shutterino.App.Devices;
using AmadeusW.Shutterino.Arduino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AmadeusW.Shutterino.App
{
    public class ShutterinoLogic
    {
        DPhone _phone = new DPhone();
        DLocation _location = new DLocation();
        DAccelerometer _accelerometer = new DAccelerometer();
        DCamera _camera = new DCamera();
        DOrientation _orientation = new DOrientation();
        ArduinoConnection _arduino = null;

        public CoreDispatcher Dispatcher { get; }
        public CaptureElement CameraPreviewControl { get; set; }
        public double Precision { get; private set; } = LOW_PRECISION;

        public static ShutterinoLogic Instance { get; private set; }

        private DispatcherTimer _photoTakingTimer;
        private DateTime lastPhotoTime;
        private bool _takingPhotos;

        public const double HIGH_PRECISION = 0.01;
        public const double LOW_PRECISION = 0.05;
        public const double HINT_PRECISION = 0.1;

        public ShutterinoLogic(CoreDispatcher dispatcher, CaptureElement cameraPreviewControl)
        {
            Instance = this;
            Dispatcher = dispatcher;
            CameraPreviewControl = cameraPreviewControl;
        }
        
        public async Task CleanUpAsync()
        {
            await Task.WhenAll(
                _phone.CleanUpAsync(),
                _location.CleanUpAsync(),
                _accelerometer.CleanUpAsync(),
                _camera.CleanUpAsync(),
                _orientation.CleanUpAsync()
            );
            _photoTakingTimer.Stop();
        }

        public async Task InitializeAsync()
        {
            await Task.WhenAll(
                _phone.InitializeAsync(),
                _location.InitializeAsync(),
                _accelerometer.InitializeAsync(),
                _camera.InitializeAsync(),
                _orientation.InitializeAsync()
            );
            _photoTakingTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _photoTakingTimer.Tick += photoTakingTimerTick;
        }

        public async Task<bool> initializeArduino(byte servoPin, byte servoIdle, byte servoOff, byte servoPressed, string host, ushort port)
        {
            _arduino = new ArduinoConnection(servoPin, servoIdle, servoOff, servoPressed);
            return await _arduino.Connect(host, port);
        }

        internal async Task<bool> disableArduino()
        {
            if (_arduino != null)
            {
                return await _arduino.Disconnect();
            }
            return false;
        }

        private async void photoTakingTimerTick(object sender, object e)
        {
            if (_takingPhotos && DateTime.UtcNow > lastPhotoTime + TimeSpan.FromSeconds(2) && IsPhotoOpportunity())
            {
                lastPhotoTime = DateTime.UtcNow;
                await TakePhoto();
            }
        }

        public async Task TakePhoto()
        {
            Task<bool> servoTask = null;
            if (_arduino != null)
            {
                servoTask = _arduino.MoveServo();
            }

            //await _camera.TakePhotoAsync();

            if (servoTask != null)
            {
                var shutterPressed = await servoTask;
                System.Diagnostics.Debug.WriteLine("Shutter: " + (shutterPressed ? "pressed" : "failure"));
            }
        }

        internal void BeginTakingPhotos()
        {
            _takingPhotos = true;
            _photoTakingTimer.Start();
        }

        internal void EndTakingPhotos()
        {
            _photoTakingTimer.Stop();
            _takingPhotos = false;
        }

        internal void UseHighPrecision()
        {
            Precision = HIGH_PRECISION;
        }

        internal void UseLowPrecision()
        {
            Precision = LOW_PRECISION;
        }

        internal void Callibrate()
        {
            _accelerometer.Callibrate();
        }

        private bool IsPhotoOpportunity()
        {
            return _accelerometer.DeltaRoll < Precision
                && _accelerometer.DeltaPitch < Precision
                && _accelerometer.DeltaYaw < Precision;
        }
    }
}
