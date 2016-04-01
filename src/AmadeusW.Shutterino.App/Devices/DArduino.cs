using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Devices
{
    public class DArduino : Device
    {
        public static DArduino Instance { get; private set; }
        public string HostName { get; internal set; }
        public string PortNumber { get; internal set; }
        public byte PositionOff { get; internal set; } = 20;
        public byte PositionIdle { get; internal set; } = 30;
        public byte PositionReady { get; internal set; } = 40;
        public byte PositionDepressed { get; internal set; } = 50;

        public override string ToString() => "Arduino";

        public DArduino() : base()
        {
            Instance = this;
            IsAvailable = true;
        }

        public override async Task ActivateAsync()
        {
            if (!IsAvailable || _isActuallyActive)
                return;

            _isActuallyActive = true;
        }

        public override async Task CleanupAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task DeactivateAsync()
        {
            if (!IsAvailable || !_isActuallyActive)
                return;

            _isActuallyActive = false;
        }

        public override async Task InitializeAsync()
        {
            
        }
    }
}
