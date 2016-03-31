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
        public DLocation() : base()
        {
            Instance = this;
        }

        public async override Task DeactivateAsync()
        {
            if (!IsAvailable || !IsActive)
                return;

            if (_geoLocator != null)
            {
                _geoLocator.PositionChanged -= _geoLocator_PositionChanged;
                _geoLocator = null;
            }
            IsActive = false;
        }

        public async override Task ActivateAsync()
        {
            if (!IsAvailable || IsActive)
                return;

            _geoLocator = new Geolocator();
            _geoLocator.ReportInterval = 2000;
            _geoLocator.DesiredAccuracy = PositionAccuracy.High;
            _geoLocator.PositionChanged += _geoLocator_PositionChanged;
            IsActive = true;
        }

        private void _geoLocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            CurrentLocation = args.Position.Coordinate.Point;
        }

        public override async Task InitializeAsync()
        {
            await Geolocator.RequestAccessAsync();
            IsAvailable = true;
        }

        public override async Task CleanupAsync()
        {
            IsAvailable = false;
        }
    }
}
