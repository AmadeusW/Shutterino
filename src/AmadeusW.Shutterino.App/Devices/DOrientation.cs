using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace AmadeusW.Shutterino.App.Devices
{
    public class DOrientation : Device
    {
        private readonly SimpleOrientationSensor _orientationSensor = SimpleOrientationSensor.GetDefault();

        public SimpleOrientation DeviceOrientation { get; private set; } = SimpleOrientation.NotRotated;

        public static DOrientation Instance { get; private set; }
        public DOrientation() : base()
        {
            Instance = this;
        }

        public override async Task CleanUpAsync()
        {
            if (_orientationSensor != null)
            {
                _orientationSensor.OrientationChanged -= OrientationSensor_OrientationChanged;
            }
        }

        public override async Task<bool> InitializeAsync()
        {
            if (_orientationSensor != null)
            {
                _orientationSensor.OrientationChanged += OrientationSensor_OrientationChanged;
                IsAvailable = true;
                return true;
            }
            return false;
        }

        private void OrientationSensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            if (args.Orientation != SimpleOrientation.Faceup && args.Orientation != SimpleOrientation.Facedown)
            {
                DeviceOrientation = args.Orientation;
            }
        }
    }
}
