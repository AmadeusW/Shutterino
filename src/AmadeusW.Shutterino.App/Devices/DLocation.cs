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

        public async override Task CleanUpAsync()
        {
            if (_geoLocator != null)
            {
                _geoLocator.PositionChanged -= _geoLocator_PositionChanged;
                _geoLocator = null;
            }
        }

        public async override Task<bool> InitializeAsync()
        {
            await Geolocator.RequestAccessAsync();
            _geoLocator = new Geolocator();
            _geoLocator.ReportInterval = 2000;
            _geoLocator.DesiredAccuracy = PositionAccuracy.High;
            _geoLocator.PositionChanged += _geoLocator_PositionChanged;
            IsAvailable = true;
            return true;
        }

        private void _geoLocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            CurrentLocation = args.Position.Coordinate.Point;
        }
    }
}
