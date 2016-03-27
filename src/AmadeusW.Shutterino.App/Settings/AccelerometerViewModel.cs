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
            set { _precision = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Required difference in ROLL reading between consecutive shots
        /// </summary>
        public double OffsetRoll
        {
            get { return _offsetRoll; }
            set { _offsetRoll = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Required difference in TILT reading between consecutive shots
        /// </summary>
        public double OffsetTilt
        {
            get { return _offsetTilt; }
            set { _offsetTilt = value; NotifyPropertyChanged(); }
        }

        #region Backing Fields

        private double _precision;
        private double _offsetRoll;
        private double _offsetTilt;

        #endregion

    }
}
