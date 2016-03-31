using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace AmadeusW.Shutterino.App.Devices
{
    public class DCamera : Device
    {
        // MediaCapture and its state variables
        private MediaCapture _mediaCapture;
        private bool _isPreviewing;
        private bool _isRecording;

        private bool _externalCamera;
        private bool _mirroringPreview;

        private int _savedPhotosCount = 0;

        public static DCamera Instance { get; private set; }
        public DCamera() : base()
        {
            Instance = this;
        }

        public async override Task DeactivateAsync()
        {
            if (!IsAvailable || !_isActuallyActive)
                return;

            if (_isPreviewing)
            {
                // The call to stop the preview is included here for completeness, but can be
                // safely removed if a call to MediaCapture.Dispose() is being made later,
                // as the preview will be automatically stopped at that point
                await StopPreviewAsync();
            }
            _isActuallyActive = false;
        }

        public async override Task CleanupAsync()
        {
            if (!IsAvailable)
                return;

            IsAvailable = false;
            if (_mediaCapture != null)
            {
                _mediaCapture.RecordLimitationExceeded -= MediaCapture_RecordLimitationExceeded;
                _mediaCapture.Failed -= MediaCapture_Failed;
                _mediaCapture.Dispose();
                _mediaCapture = null;
            }
        }

        /// <summary>
        /// Initializes the MediaCapture, registers events, gets camera device information for mirroring and rotating, starts preview and unlocks the UI
        /// </summary>
        /// <returns></returns>
        public async override Task InitializeAsync()
        {
            if (IsAvailable || _mediaCapture != null)
                return;

            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the back-mounted camera
            DeviceInformation cameraDevice =
                allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null &&
                x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back);

            // If there is no camera on the specified panel, get any camera
            cameraDevice = cameraDevice ?? allVideoDevices.FirstOrDefault();

            if (cameraDevice == null)
            {
                Debug.WriteLine("No camera available");
                return;
            }

            // Create MediaCapture and its settings
            _mediaCapture = new MediaCapture();

            // Register for a notification when video recording has reached the maximum time and when something goes wrong
            _mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;
            _mediaCapture.Failed += MediaCapture_Failed;

            var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

            // Initialize MediaCapture
            try
            {
                await _mediaCapture.InitializeAsync(settings);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("The app was denied access to the camera");
                return;
            }

            // Figure out where the camera is located
            if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
            {
                // No information on the location of the camera, assume it's an external camera, not integrated on the device
                _externalCamera = true;
            }
            else
            {
                // Camera is fixed on the device
                _externalCamera = false;

                // Only mirror the preview if the camera is on the front panel
                _mirroringPreview = (cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
            }

            IsAvailable = true;
        }

        public override async Task ActivateAsync()
        {
            if (!IsAvailable || _isActuallyActive)
                return;

            // Activate only if user wants to
            if (IsActive)
            {
                _isActuallyActive = true;
                await StartPreviewAsync();
            }
        }

        private async Task StartPreviewAsync()
        {
            // Set the preview source in the UI and mirror it if necessary
            ShutterinoLogic.Instance.CameraPreviewControl.Source = _mediaCapture;
            ShutterinoLogic.Instance.CameraPreviewControl.FlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            // Start the preview
            try
            {
                await _mediaCapture.StartPreviewAsync();
                _isPreviewing = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when starting the preview: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Stops the preview and deactivates a display request, to allow the screen to go into power saving modes
        /// </summary>
        /// <returns></returns>
        private async Task StopPreviewAsync()
        {
            // Stop the preview
            _isPreviewing = false;
            await _mediaCapture.StopPreviewAsync();

            // Use the dispatcher because this method is sometimes called from non-UI threads
            await ShutterinoLogic.Instance.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Cleanup the UI
                ShutterinoLogic.Instance.CameraPreviewControl.Source = null;
            });
        }

        private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            throw new NotImplementedException();
        }

        private void MediaCapture_RecordLimitationExceeded(MediaCapture sender)
        {
            throw new NotImplementedException();
        }

        public async Task TakePhotoAsync()
        {
            var stream = new InMemoryRandomAccessStream();

            try
            {
                Debug.WriteLine("Taking photo...");
                await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);
                Debug.WriteLine("Photo taken!");

                var photoOrientation = PhotoOrientation.Rotate90;// ConvertOrientationToPhotoOrientation(DOrientation.Instance.DeviceOrientation);
                _savedPhotosCount++;
                await ReencodeAndSavePhotoAsync(stream, $"Shutterino {_savedPhotosCount}.jpg", photoOrientation);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when taking a photo: {0}", ex.ToString());
            }
        }

        private static async Task ReencodeAndSavePhotoAsync(IRandomAccessStream stream, string filename, PhotoOrientation photoOrientation)
        {
            using (var inputStream = stream)
            {
                var decoder = await BitmapDecoder.CreateAsync(inputStream);

                var file = await KnownFolders.PicturesLibrary.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

                using (var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);

                    var properties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(photoOrientation, PropertyType.UInt16) } };

                    await encoder.BitmapProperties.SetPropertiesAsync(properties);
                    await encoder.FlushAsync();
                }
            }
        }
        /*
        private SimpleOrientation GetCameraOrientation()
        {
            if (_externalCamera)
            {
                // Cameras that are not attached to the device do not rotate along with it, so apply no rotation
                return SimpleOrientation.NotRotated;
            }

            // If the preview is being mirrored for a front-facing camera, then the rotation should be inverted
            if (_mirroringPreview)
            {
                // This only affects the 90 and 270 degree cases, because rotating 0 and 180 degrees is the same clockwise and counter-clockwise
                switch (DOrientation.Instance.DeviceOrientation)
                {
                    case SimpleOrientation.Rotated90DegreesCounterclockwise:
                        return SimpleOrientation.Rotated270DegreesCounterclockwise;
                    case SimpleOrientation.Rotated270DegreesCounterclockwise:
                        return SimpleOrientation.Rotated90DegreesCounterclockwise;
                }
            }

            return DOrientation.Instance.DeviceOrientation;
        }
        */

        private static PhotoOrientation ConvertOrientationToPhotoOrientation(SimpleOrientation orientation)
        {
            switch (orientation)
            {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return PhotoOrientation.Rotate90;
                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    return PhotoOrientation.Rotate180;
                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return PhotoOrientation.Rotate270;
                case SimpleOrientation.NotRotated:
                default:
                    return PhotoOrientation.Normal;
            }
        }
    }
}
