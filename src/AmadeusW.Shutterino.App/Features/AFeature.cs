using AmadeusW.Shutterino.App.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Storage;

namespace AmadeusW.Shutterino.App.Features
{
    public abstract class AFeature : PropertyChangedBase, IDisposable
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

        public async Task InitializeAsync()
        {
            try
            {
                await InitializeAsyncCore();
            }
            catch (Exception ex)
            {
                // TODO: log with App Insights
                Status = "Init error: " + ex.ToString();
                IsActive = false;
            }
        }

        public async Task CleanupAsync()
        {
            try
            {
                await CleanupAsyncCore();
            }
            catch (Exception ex)
            {
                // TODO: log with App Insights
                Status = "Cleanup error: " + ex.ToString();
                IsActive = false;
            }
        }

        public async Task ActivateAsync()
        {
            try
            {
                await ActivateAsyncCore();
            }
            catch (Exception ex)
            {
                // TODO: log with App Insights
                Status = "Activate error: " + ex.ToString();
                IsActive = false;
            }
        }

        public async Task DeactivateAsync()
        {
            try
            {
                await DeactivateAsyncCore();
            }
            catch (Exception ex)
            {
                // TODO: log with App Insights
                Status = "Deactivate error: " + ex.ToString();
                IsActive = false;
            }
        }

        protected abstract Task InitializeAsyncCore();
        protected abstract Task ActivateAsyncCore();
        protected abstract Task DeactivateAsyncCore();
        protected abstract Task CleanupAsyncCore();

        protected ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public void Dispose()
        {

        }
    }
}
