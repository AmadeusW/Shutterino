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
        public int Precision
        {
            get { return _precision; }
            set
            {
                if (_precision != value)
                {
                    _precision = value;
                    NotifyPropertyChanged();
                    _accelerometer.Precision = _precision / 100;
                }
            }
        }

        /// <summary>
        /// Required difference in ROLL reading between consecutive shots
        /// </summary>
        public int RollOffset
        {
            get { return _rollOffset; }
            set
            {
                if (_rollOffset != value)
                {
                    _rollOffset = value;
                    NotifyPropertyChanged();
                    _accelerometer.RollOffset = _rollOffset / 100;
                }
            }
        }

        /// <summary>
        /// Required difference in TILT reading between consecutive shots
        /// </summary>
        public int PitchOffset
        {
            get { return _pitchOffset; }
            set
            {
                if (_pitchOffset != value)
                {
                    _pitchOffset = value;
                    NotifyPropertyChanged();
                    _accelerometer.PitchOffset = _pitchOffset / 100;
                }
            }
        }

        /// <summary>
        /// Prevents taking pictures too often
        /// </summary>
        public int RateLimiter
        {
            get { return _rateLimiter; }
            set
            {
                if (_rateLimiter != value)
                {
                    _rateLimiter = value;
                    NotifyPropertyChanged();
                    _accelerometer.RateLimiter = (int)(_rateLimiter * TimeSpan.TicksPerMillisecond);
                }
            }
        }

        public AccelerometerViewModel() : base(DAccelerometer.Instance)
        {
            _available = _accelerometer.IsAvailable;
            _active = _accelerometer.IsActive;
            _precision = (int)(_accelerometer.Precision * 100);
            _rollOffset = (int)(_accelerometer.RollOffset * 100);
            _pitchOffset = (int)(_accelerometer.PitchOffset * 100);
            _rateLimiter = (int)(_accelerometer.RateLimiter / TimeSpan.TicksPerMillisecond);
        }

        #region Backing Fields

        private int _precision;
        private int _rollOffset;
        private int _pitchOffset;
        private int _rateLimiter;

        #endregion

    }
}
