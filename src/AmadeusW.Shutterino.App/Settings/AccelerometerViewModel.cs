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
        DAccelerometer _accelerometer => _device as DAccelerometer;

        /// <summary>
        /// Maximum allowed error on the accelerometer axes
        /// </summary>
        public double Precision
        {
            get { return _precision; }
            set { _precision = value; NotifyPropertyChanged(); _accelerometer.Precision = _precision / 100; }
        }

        /// <summary>
        /// Required difference in ROLL reading between consecutive shots
        /// </summary>
        public double RollOffset
        {
            get { return _rollOffset; }
            set { _rollOffset = value; NotifyPropertyChanged(); _accelerometer.RollOffset = _rollOffset / 100;  }
        }

        /// <summary>
        /// Required difference in TILT reading between consecutive shots
        /// </summary>
        public double PitchOffset
        {
            get { return _pitchOffset; }
            set { _pitchOffset = value; NotifyPropertyChanged(); _accelerometer.PitchOffset = _pitchOffset / 100; }
        }

        public AccelerometerViewModel() : base(DAccelerometer.Instance)
        {
            _available = _accelerometer.IsAvailable;
            _active = _accelerometer.IsActive;
            _precision = _accelerometer.Precision * 100;
            _rollOffset = _accelerometer.RollOffset * 100;
            _pitchOffset = _accelerometer.PitchOffset * 100;
        }

        #region Backing Fields

        private double _precision;
        private double _rollOffset;
        private double _pitchOffset;

        #endregion

    }
}
