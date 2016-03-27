using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Settings
{
    public class ShutterinoModuleViewModel : PropertyChangedBase
    {
        public ShutterinoModuleViewModel()
        {

        }

        /// <summary>
        /// Whether this module has been initialized and can be interacted with.
        /// Usually, when a device lacks some functionality, its module is not initialized
        /// </summary>
        public bool Initialized
        {
            get { return _initialized; }
            set { _initialized = value; NotifyPropertyChanged(); }
        }
        
        /// <summary>
        /// Whether this module is active
        /// </summary>
        public bool Active
        {
            get { return _active; }
            set { _active = value; NotifyPropertyChanged(); }
        }

        #region Backing fields

        private bool _initialized;
        private bool _active;

        #endregion
    }
}
