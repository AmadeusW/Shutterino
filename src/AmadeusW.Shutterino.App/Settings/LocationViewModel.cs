using AmadeusW.Shutterino.App.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Settings
{
    public class LocationViewModel : ShutterinoModuleViewModel
    {
        LocationFeature _location => Device as LocationFeature;

        public LocationViewModel() : base(LocationFeature.Instance)
        {
            Offset = _location.Offset;
        }

        /// <summary>
        /// Required difference in the position reading between consecutive shots
        /// </summary>
        public double Offset
        {
            get { return _offset; }
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    NotifyPropertyChanged();
                    _location.Offset = _offset;
                }
            }
        }
        
        #region Backing Fields

        private double _precision;
        private double _offset;

        #endregion

    }
}
