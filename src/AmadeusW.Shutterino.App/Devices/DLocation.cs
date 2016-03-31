using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace AmadeusW.Shutterino.App.Devices
{
    public class DLocation : Device
    {
        public Geopoint CurrentLocation { get; private set; }
        private Geolocator _geoLocator;

        public static DLocation Instance { get; private set; }

        private double _offset = 50; // Load from settings
        public double Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                if (_geoLocator != null)
                {
                    _geoLocator.MovementThreshold = _offset;
                }
            }
        }

        public DLocation() : base()
        {
            Instance = this;
        }

        public async override Task DeactivateAsync()
        {
            if (!IsAvailable || !_isActuallyActive)
                return;

            if (_geoLocator != null)
            {
                _geoLocator.PositionChanged -= _geoLocator_PositionChanged;
                _geoLocator = null;
            }
            _isActuallyActive = false;
        }

        public async override Task ActivateAsync()
        {
            if (!IsAvailable || _isActuallyActive)
                return;

            // Activate only if user wants to
            if (IsActive)
            {
                _geoLocator = new Geolocator();
                //_geoLocator.ReportInterval = 2000;
                _geoLocator.MovementThreshold = Offset;
                _geoLocator.DesiredAccuracy = PositionAccuracy.High;
                _geoLocator.PositionChanged += _geoLocator_PositionChanged;
                _isActuallyActive = true;
            }
        }

        private async void _geoLocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            CurrentLocation = args.Position.Coordinate.Point;
            // This event is fired whenever user crosses the thereshold, so let's snap the photo
            try
            {
                await ShutterinoLogic.Instance.TakePhoto();
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("error taking photo from GeoLocatior");
            }
        }

        public override async Task InitializeAsync()
        {
            await Geolocator.RequestAccessAsync();
            IsAvailable = true;
        }

        public override async Task CleanupAsync()
        {
            IsAvailable = false;
            await DeactivateAsync();
        }
    }
}
