using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Settings
{
    public class LocationViewModel : ShutterinoModuleViewModel
    {
        /// <summary>
        /// Maximum allowed error on the location reading
        /// </summary>
        public double Precision
        {
            get { return _precision; }
            set { _precision = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Required difference in the position reading between consecutive shots
        /// </summary>
        public double Offset
        {
            get { return _offset; }
            set { _offset = value; NotifyPropertyChanged(); }
        }
        
        #region Backing Fields

        private double _precision;
        private double _offset;

        #endregion

    }
}
