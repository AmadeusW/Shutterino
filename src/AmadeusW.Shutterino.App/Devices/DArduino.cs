using AmadeusW.Shutterino.Arduino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Devices
{
    public class DArduino : Device
    {
        private ArduinoConnection _arduino;

        public static DArduino Instance { get; private set; }
        public string HostName { get; internal set; } = "192.168.1.113";
        public byte PinNumber { get; internal set; } = 4;
        public ushort PortNumber { get; internal set; } = 3030;
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

            if (IsActive)
            {
                await _arduino.Connect(HostName, PortNumber);

                _isActuallyActive = true;
            }
        }

        public override async Task CleanupAsync()
        {

        }

        public override async Task DeactivateAsync()
        {
            if (!IsAvailable || !_isActuallyActive)
                return;

            await _arduino.Disconnect();

            _isActuallyActive = false;
        }

        public override async Task InitializeAsync()
        {
            _arduino = new ArduinoConnection(PinNumber, PositionOff, PositionIdle, PositionReady, PositionDepressed);
            return;
        }

        internal async Task<bool> MoveServo()
        {
            if (!_isActuallyActive)
                return false;

            return await _arduino.MoveServo();
        }
    }
}
