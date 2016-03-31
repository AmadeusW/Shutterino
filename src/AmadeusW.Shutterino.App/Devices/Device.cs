using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace AmadeusW.Shutterino.App.Devices
{
    public abstract class Device : IDisposable
    {
        public bool IsAvailable { get; protected set; } = false;
        public bool IsActive { get; protected set; } = false;
        public string Status { get; protected set; }

        public abstract Task InitializeAsync();
        public abstract Task ActivateAsync();
        public abstract Task DeactivateAsync();
        public abstract Task CleanupAsync();

        public void Dispose()
        {

        }
    }
}
