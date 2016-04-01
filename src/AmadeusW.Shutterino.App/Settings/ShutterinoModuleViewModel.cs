using AmadeusW.Shutterino.App.Features;
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
        private AFeature device;

        public ShutterinoModuleViewModel(AFeature device)
        {
            if (device == null)
                return; // Temporarily, until I create all devices
                //throw new ArgumentNullException(nameof(device));

            Device = device;
            Available = Device.IsAvailable;
            Active = Device.IsActive;
            ToggleCommand = new ToggleCommand(this);
            Device.PropertyChanged += ShutterinoModuleViewModel_PropertyChanged;
        }

        private void ShutterinoModuleViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Device.IsAvailable))
                Available = Device.IsAvailable;
            if (e.PropertyName == nameof(Device.IsActive))
                Active = Device.IsActive;
            if (e.PropertyName == nameof(Device.Status))
                Status = Device.Status;
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
                    Device.IsActive = _active;
                    NotifyPropertyChanged();
                }
            }
        }

        protected AFeature Device
        {
            get
            {
                return device;
            }

            set
            {
                device = value;
            }
        }

        public string Status { get; internal set; }

        #region Backing fields

        protected bool _available;
        protected bool _active;

        #endregion
    }
}
