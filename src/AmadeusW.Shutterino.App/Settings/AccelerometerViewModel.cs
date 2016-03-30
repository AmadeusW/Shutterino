using AmadeusW.Shutterino.App.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Settings
{
    public class AccelerometerViewModel : ShutterinoModuleViewModel
    {
        /// <summary>
        /// Maximum allowed error on the accelerometer axes
        /// </summary>
        public double Precision
        {
            get { return _precision; }
            set { _precision = value; NotifyPropertyChanged(); device.Precision = Precision; }
        }

        /// <summary>
        /// Required difference in ROLL reading between consecutive shots
        /// </summary>
        public double RollOffset
        {
            get { return _rollOffset; }
            set { _rollOffset = value; NotifyPropertyChanged(); device.RollOffset = RollOffset;  }
        }

        /// <summary>
        /// Required difference in TILT reading between consecutive shots
        /// </summary>
        public double PitchOffset
        {
            get { return _pitchOffset; }
            set { _pitchOffset = value; NotifyPropertyChanged(); device.PitchOffset = PitchOffset; }
        }

        DAccelerometer device;

        public AccelerometerViewModel()
        {
            device = Devices.DAccelerometer.Instance;
            _initialized = device.IsAvailable;
            _active = device.IsAvailable;
            _precision = device.Precision;
            _rollOffset = device.RollOffset;
            _pitchOffset = device.PitchOffset;
        }

        #region Backing Fields

        private double _precision;
        private double _rollOffset;
        private double _pitchOffset;

        #endregion

    }
}
