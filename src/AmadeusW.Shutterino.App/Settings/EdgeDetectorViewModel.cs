using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Settings
{
    public class EdgeDetectorViewModel : ShutterinoModuleViewModel
    {
        public EdgeDetectorViewModel() : base(null)
        {

        }

        /// <summary>
        /// Maximum allowed error on the edge detector
        /// </summary>
        public double Param
        {
            get { return _precision; }
            set { _precision = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Required shift on the X axis between consecutive shots
        /// </summary>
        public double OffsetX
        {
            get { return _offsetX; }
            set { _offsetX = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Required shift on the Y axis between consecutive shots
        /// </summary>
        public double OffsetTilt
        {
            get { return _offsetY; }
            set { _offsetY = value; NotifyPropertyChanged(); }
        }

        #region Backing Fields

        private double _precision;
        private double _offsetX;
        private double _offsetY;

        #endregion

    }
}
