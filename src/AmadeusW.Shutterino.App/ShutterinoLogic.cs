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
        DTimer _timer = new DTimer();
        ArduinoConnection _arduino = null;

        public CoreDispatcher Dispatcher { get; }
        public CaptureElement CameraPreviewControl { get; set; }

        public static ShutterinoLogic Instance { get; private set; }
        public bool TakesPhotos { get; internal set; }

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

            _initialized = true;
        }

        public async Task ActivateAsync()
        {
            await Task.WhenAll(
                _phone.ActivateAsync(),
                _location.ActivateAsync(),
                _accelerometer.ActivateAsync(),
                _camera.ActivateAsync()
            );
        }

        public async Task DeactivateAsync()
        {
            await Task.WhenAll(
                _phone.DeactivateAsync(),
                _location.DeactivateAsync(),
                _accelerometer.DeactivateAsync(),
                _camera.DeactivateAsync()
            );
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

        private async Task TakePhoto()
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

        internal async Task SuggestPhotoOpportunity(Device sender)
        {
            // log sender.ToString();
            try
            {
                await TakePhoto();
            }
            catch (Exception ex)
            {
                // TODO: LOG
                sender.Status = ex.ToString();
            }
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
