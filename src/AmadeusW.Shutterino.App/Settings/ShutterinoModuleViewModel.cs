using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmadeusW.Shutterino.App.Settings
{
    public class ShutterinoModuleViewModel : PropertyChangedBase
    {
        protected Devices.Device _device;

        public ShutterinoModuleViewModel(Devices.Device device)
        {
            if (device == null)
                return; // Temporarily, until I create all devices
                //throw new ArgumentNullException(nameof(device));

            _device = device;
            Available = _device.IsAvailable;
            Active = _device.IsActive;
            _device.PropertyChanged += ShutterinoModuleViewModel_PropertyChanged;
        }

        private void ShutterinoModuleViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_device.IsAvailable))
                Available = _device.IsAvailable;
            if (e.PropertyName == nameof(_device.IsActive))
                Active = _device.IsActive;
        }

        public ICommand ToggleCommand { get; }

        /// <summary>
        /// Whether this module has been initialized and can be interacted with.
        /// Usually, when a device lacks some functionality, its module is not initialized
        /// </summary>
        public bool Available
        {
            get { return _available; }
            set
            {
                if (_available != value)
                {
                    _available = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Whether this module is active
        /// </summary>
        public bool Active
        {
            get { return _active; }
            set
            {
                if (_active != value)
                {
                    _active = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #region Backing fields

        protected bool _available;
        protected bool _active;

        #endregion
    }
}
