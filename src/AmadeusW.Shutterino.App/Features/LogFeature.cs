using AmadeusW.Shutterino.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace AmadeusW.Shutterino.App.Features
{
    public class LogFeature : AFeature
    {
        public static LogFeature Instance { get; private set; }
        public override string ToString() => "Logger";

        public string LastSync { get; set; }
        public int PhotoCount { get; set; }

        StorageFolder _localFolder = ApplicationData.Current.LocalFolder;
        private List<string> loggedData;
        string _connectionString;

        public LogFeature() : base()
        {
            Instance = this;
            IsAvailable = true;

            loggedData = new List<string>();

            var resources = new Windows.ApplicationModel.Resources.ResourceLoader("Resources");
            _connectionString = resources.GetString("AzureConnectionString");

            LastSync = (string)(_localSettings.Values["log-LogStart"] ?? "never");
            PhotoCount = (int)(_localSettings.Values["log-PhotoCount"] ?? 0);
            IsActive = (bool)(_localSettings.Values["log-IsActive"] ?? false);
        }

        public void LogPhotoTaken(string reason)
        {
            if (!IsActive)
                return;

            try
            {
                if (String.IsNullOrEmpty(reason))
                    reason = "Manual";

                var date = DateTime.Now;
                var cameraFileName = CameraFeature.Instance.PhotoCount;

                var accelerometer = AccelerometerFeature.Instance.IsActive ? "true" : "false";
                var pitch = AccelerometerFeature.Instance.Pitch;
                var roll = AccelerometerFeature.Instance.Roll;
                var pitchOffset = AccelerometerFeature.Instance.PitchOffset;
                var rollOffset = AccelerometerFeature.Instance.RollOffset;
                var targetPitch = AccelerometerFeature.Instance.TargetPitch;
                var targetRoll = AccelerometerFeature.Instance.TargetRoll;
                var precision = AccelerometerFeature.Instance.Precision;
                var location = LocationFeature.Instance.IsActive ? "true" : "false";
                var distance = LocationFeature.Instance.Offset;
                var longitude = LocationFeature.Instance.CurrentLocation?.Position.Longitude ?? 0d;
                var latitude = LocationFeature.Instance.CurrentLocation?.Position.Latitude ?? 0d;
                var altitude = LocationFeature.Instance.CurrentLocation?.Position.Altitude ?? 0d;
                var camera = CameraFeature.Instance.IsActive ? "true" : "false";
                var timer = TimerFeature.Instance.IsActive ? "true" : "false";
                var arduino = ArduinoFeature.Instance.IsActive ? "true" : "false";

                if (PhotoCount == 0)
                {
                    appendHeader();
                }
                loggedData.Add(
                    $"{date},{cameraFileName},{reason},{accelerometer},{pitch},{roll},{pitchOffset},{rollOffset},{targetPitch},{targetRoll},{precision},{location},{distance},{longitude},{latitude},{altitude},{camera},{timer},{arduino}\n"
                );
                PhotoCount++;
            }
            catch (Exception ex)
            {
                //TODO: log
                Status = ex.ToString();
            }
        }

        public async Task Upload()
        {
            try
            {
                await SaveFile();
                var currentTimeString = DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss");
                var cloudFileName = $"log.{currentTimeString}.csv";
                if (String.IsNullOrEmpty(_connectionString))
                    throw new InvalidOperationException("Please provide Azure Storage Account connection string in 'AzureConnectionString' resource");

                var path = await FileUploader.UploadFile(
                    _connectionString.ToString(),
                    cloudFileName,
                    await _localFolder.GetFileAsync("shutterino.csv")
                    );
                Status = $"Uploaded log to {path}";
                // Remove file
                LastSync = DateTime.Now.ToString();
                PhotoCount = 0;
            }
            catch (Exception ex)
            {
                //TODO: log
                Status = ex.ToString();
            }
        }

        private void appendHeader()
        {
            loggedData.Add(
                $"date,cameraFileName,reason,accelerometer,pitch,roll,pitchOffset,rollOffset,targetPitch,targetRoll,precision,location,distance,longitude,latitude,altitude,camera,timer,arduino\n"
            );
        }

        public override async Task ActivateAsync()
        {
            if (!IsAvailable || _isActuallyActive)
                return;

            // Activate only if user wants to
            if (IsActive)
            {
                _isActuallyActive = true;
            }
        }

        public override async Task CleanupAsync()
        {
            
            _localSettings.Values["log-LogStart"] = LastSync;
            _localSettings.Values["log-PhotoCount"] = PhotoCount;
            _localSettings.Values["log-IsActive"] = IsActive;
        }

        private async Task SaveFile()
        {
            try
            {
                StorageFile logFile = await _localFolder.CreateFileAsync("shutterino.csv",
                    CreationCollisionOption.OpenIfExists);
                await FileIO.AppendLinesAsync(logFile, loggedData);
                loggedData = new List<string>();
            }
            catch (Exception ex)
            {
                // TODO: log
                Status = ex.ToString();
            }
        }

        public override async Task DeactivateAsync()
        {
            if (!IsAvailable || !_isActuallyActive)
                return;

            SaveFile();

            _isActuallyActive = false;
        }

        public override async Task InitializeAsync()
        {
            // nothing to initialize
        }
    }
}
