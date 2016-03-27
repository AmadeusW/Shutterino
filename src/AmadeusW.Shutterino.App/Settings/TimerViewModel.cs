using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Settings
{
    public class TimerViewModel : ShutterinoModuleViewModel
    {
        /// <summary>
        /// In which pattern the shutter should be pressed
        /// </summary>
        public string Pattern
        {
            get { return _pattern; }
            set { _pattern = value; NotifyPropertyChanged(); }
        }

        #region Backing Fields

        private string _pattern;

        #endregion

    }
}
