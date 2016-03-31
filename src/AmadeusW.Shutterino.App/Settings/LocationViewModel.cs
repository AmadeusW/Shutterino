using AmadeusW.Shutterino.App.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Settings
{
    public class LocationViewModel : ShutterinoModuleViewModel
    {
        DLocation _location => _device as DLocation;

        public LocationViewModel() : base(Devices.DLocation.Instance)
        {

        }

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
            set { _offset = value; NotifyPropertyChanged(); _location.Offset = _offset; }
        }
        
        #region Backing Fields

        private double _precision;
        private double _offset;

        #endregion

    }
}
