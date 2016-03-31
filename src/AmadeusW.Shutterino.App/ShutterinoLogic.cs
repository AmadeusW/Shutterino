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
        ArduinoConnection _arduino = null;

        public CoreDispatcher Dispatcher { get; }
        public CaptureElement CameraPreviewControl { get; set; }

        public static ShutterinoLogic Instance { get; private set; }

        private DispatcherTimer _photoTakingTimer;
        private DateTime lastPhotoTime;
        private bool _takingPhotos;
        private bool _initialized;

        internal static ShutterinoLogic Get(CoreDispatcher dispatcher, CaptureElement previewControl)
        {
            if (Instance != null)
            {
                Instance.CameraPreviewControl = previewControl;
                return Instance;
            }
            else
            {
                return new ShutterinoLogic(dispatcher, previewControl);
            }
        }

        private ShutterinoLogic(CoreDispatcher dispatcher, CaptureElement cameraPreviewControl)
        {
            Instance = this;
            Dispatcher = dispatcher;
            CameraPreviewControl = cameraPreviewControl;
        }
        
        public async Task CleanUpAsync()
        {
            if (!_initialized)
                return;

            await DeactivateAsync();
            await Task.WhenAll(
                _phone.CleanupAsync(),
                _location.CleanupAsync(),
                _accelerometer.CleanupAsync(),
                _camera.CleanupAsync()
            );
            _initialized = false;
        }

        public async Task InitializeAsync()
        {
            if (_initialized)
                return;
            
            await Task.WhenAll(
                _phone.InitializeAsync(),
                _location.InitializeAsync(),
                _accelerometer.InitializeAsync(),
                _camera.InitializeAsync()
            );
            _photoTakingTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _photoTakingTimer.Tick += photoTakingTimerTick;

            _initialized = true;
        }

        public async Task ActivateAsync()
        {
            // nothing to activate
        }

        public async Task DeactivateAsync()
        {
            EndTakingPhotos();
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

            await _camera.TakePhotoAsync();

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
            if (DAccelerometer.Instance != null)
                DAccelerometer.Instance.Precision = DAccelerometer.HIGH_PRECISION;
        }

        internal void UseLowPrecision()
        {
            if (DAccelerometer.Instance != null)
                DAccelerometer.Instance.Precision = DAccelerometer.LOW_PRECISION;
        }

        internal void Callibrate()
        {
            _accelerometer.Callibrate();
        }

        private bool IsPhotoOpportunity()
        {
            return (Devices.DAccelerometer.Instance?.IsPhotoOpportunity()).Value;
        }
    }
}
