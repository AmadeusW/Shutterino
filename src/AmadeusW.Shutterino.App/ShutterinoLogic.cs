using AmadeusW.Shutterino.App.Features;
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
        PhoneFeature _phone = new PhoneFeature();
        LocationFeature _location = new LocationFeature();
        AccelerometerFeature _accelerometer = new AccelerometerFeature();
        CameraFeature _camera = new CameraFeature();
        TimerFeature _timer = new TimerFeature();
        ArduinoFeature _arduino = new ArduinoFeature();
        LogFeature _log = new LogFeature();

        public CoreDispatcher Dispatcher { get; }
        public CaptureElement CameraPreviewControl { get; set; }

        public static ShutterinoLogic Instance { get; private set; }
        public bool TakesPhotos { get; internal set; }

        private bool _initialized;
        private Func<Task> PhotoTakenCallback;

        internal static ShutterinoLogic Get(CoreDispatcher dispatcher, CaptureElement previewControl, Func<Task> photoTakenCallback)
        {
            if (Instance != null)
            {
                Instance.CameraPreviewControl = previewControl;
                Instance.PhotoTakenCallback = photoTakenCallback;
                return Instance;
            }
            else
            {
                return new ShutterinoLogic(dispatcher, previewControl, photoTakenCallback);
            }
        }

        private ShutterinoLogic(CoreDispatcher dispatcher, CaptureElement cameraPreviewControl, Func<Task> photoTakenCallback)
        {
            Instance = this;
            Dispatcher = dispatcher;
            CameraPreviewControl = cameraPreviewControl;
            PhotoTakenCallback = photoTakenCallback;
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
                _camera.CleanupAsync(),
                _timer.CleanupAsync(),
                _arduino.CleanupAsync(),
                _log.CleanupAsync()
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
                _camera.InitializeAsync(),
                _timer.InitializeAsync(),
                _arduino.InitializeAsync(),
                _log.InitializeAsync()
            );

            _initialized = true;
        }

        public async Task ActivateAsync()
        {
            await Task.WhenAll(
                _phone.ActivateAsync(),
                _location.ActivateAsync(),
                _accelerometer.ActivateAsync(),
                _camera.ActivateAsync(),
                _timer.ActivateAsync(),
                _arduino.ActivateAsync(),
                _log.ActivateAsync()
            );
        }

        public async Task DeactivateAsync()
        {
            await Task.WhenAll(
                _phone.DeactivateAsync(),
                _location.DeactivateAsync(),
                _accelerometer.DeactivateAsync(),
                _camera.DeactivateAsync(),
                _timer.DeactivateAsync(),
                _arduino.DeactivateAsync(),
                _log.DeactivateAsync()
            );
        }

        private async Task TakePhoto(string reason)
        {
            _log.LogPhotoTaken(reason);

            Task<bool> servoTask = null;
            if (_arduino?.IsActive == true)
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

        internal async Task SuggestPhotoOpportunity(AFeature sender)
        {
            if (!TakesPhotos)
                return;

            try
            {
                // Animate the button for programatically-triggered photos.
                if (sender != null)
                    await PhotoTakenCallback?.Invoke();
            }
            catch { } // swallow 

            try
            {
                await TakePhoto(sender?.ToString());
            }
            catch (Exception ex)
            {
                // TODO: LOG
                sender.Status = ex.ToString();
            }
        }

        internal void Callibrate()
        {
            _accelerometer.Callibrate();
        }
    }
}
