using AmadeusW.Shutterino.App.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace AmadeusW.Shutterino.App.Devices
{
    public abstract class Device : PropertyChangedBase, IDisposable
    {
        private bool _isAvailable = false;
        private bool _isActive = false;
        private string _status = String.Empty;

        /// <summary>
        /// Whether the activation code has run
        /// </summary>
        protected bool _isActuallyActive = false;

        /// <summary>
        /// Whether this feature is available
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return _isAvailable;
            }
            protected set
            {
                if (value != _isAvailable)
                {
                    _isAvailable = value;
                    NotifyPropertyChanged();
                }
            }
        }
                
        /// <summary>
        /// Whether the user wants this feature to be active
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (value != _isActive)
                {
                    _isActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (value != _status)
                {
                    _status = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public abstract Task InitializeAsync();
        public abstract Task ActivateAsync();
        public abstract Task DeactivateAsync();
        public abstract Task CleanupAsync();

        public void Dispose()
        {

        }
    }
}
