using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace AmadeusW.Shutterino.App.Features
{
    public class LocationFeature : AFeature
    {
        public Geopoint CurrentLocation { get; private set; }
        private Geolocator _geoLocator;

        public static LocationFeature Instance { get; private set; }

        private double _offset;
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

        public override string ToString() => "Location";

        public LocationFeature() : base()
        {
            Instance = this;

            Offset = (double)(_localSettings.Values["location-Offset"] ?? 50d);
            IsActive = (bool)(_localSettings.Values["location-IsActive"] ?? false);
        }

        protected async override Task DeactivateAsyncCore()
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

        protected async override Task ActivateAsyncCore()
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
                await ShutterinoLogic.Instance.SuggestPhotoOpportunity(this);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("error taking photo from GeoLocatior");
            }
        }

        protected override async Task InitializeAsyncCore()
        {
            await Geolocator.RequestAccessAsync();
            IsAvailable = true;
        }

        protected override async Task CleanupAsyncCore()
        {
            IsAvailable = false;
            await DeactivateAsyncCore();

            _localSettings.Values["location-Offset"] = Offset;
            _localSettings.Values["location-IsActive"] = IsActive;
        }
    }
}
