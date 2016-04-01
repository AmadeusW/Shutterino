using AmadeusW.Shutterino.Arduino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Features
{
    public class ArduinoFeature : AFeature
    {
        private ArduinoConnection _arduino;

        public static ArduinoFeature Instance { get; private set; }
        public string HostName { get; internal set; }
        public byte PinNumber { get; internal set; }
        public ushort PortNumber { get; internal set; }
        public byte PositionOff { get; internal set; }
        public byte PositionIdle { get; internal set; }
        public byte PositionReady { get; internal set; }
        public byte PositionDepressed { get; internal set; }
        public int PressTime { get; internal set; }

        public override string ToString() => "Arduino";

        public ArduinoFeature() : base()
        {
            Instance = this;
            IsAvailable = true;

            HostName = (string)(_localSettings.Values["arduino-HostName"] ?? "192.168.1.113");
            PinNumber = (byte)(_localSettings.Values["arduino-PinNumber"] ?? (byte)4);
            PortNumber = (ushort)(_localSettings.Values["arduino-PortNumber"] ?? (ushort)3030);
            PositionOff = (byte)(_localSettings.Values["arduino-PositionOff"] ?? (byte)20);
            PositionIdle = (byte)(_localSettings.Values["arduino-PositionIdle"] ?? (byte)30);
            PositionReady = (byte)(_localSettings.Values["arduino-PositionReady"] ?? (byte)40);
            PositionDepressed = (byte)(_localSettings.Values["arduino-PositionDepressed"] ?? (byte)50);
            PressTime = (int)(_localSettings.Values["arduino-PressTime"] ?? 100);
            IsActive = (bool)(_localSettings.Values["arduino-IsActive"] ?? false);
        }

        public override async Task ActivateAsync()
        {
            if (!IsAvailable || _isActuallyActive)
                return;

            if (IsActive)
            {
                _arduino.UpdateSettings(PinNumber, PositionOff, PositionIdle, PositionReady, PositionDepressed, PressTime);
                await _arduino.Connect(HostName, PortNumber);

                _isActuallyActive = true;
            }
        }

        public override async Task CleanupAsync()
        {
            _localSettings.Values["arduino-HostName"] = HostName;
            _localSettings.Values["arduino-PinNumber"] = PinNumber;
            _localSettings.Values["arduino-PortNumber"] = PortNumber;
            _localSettings.Values["arduino-PositionOff"] = PositionOff;
            _localSettings.Values["arduino-PositionIdle"] = PositionIdle;
            _localSettings.Values["arduino-PositionReady"] = PositionReady;
            _localSettings.Values["arduino-PositionDepressed"] = PositionDepressed;
            _localSettings.Values["arduino-PressTime"] = PressTime;
            _localSettings.Values["arduino-IsActive"] = IsActive;
        }

        public override async Task DeactivateAsync()
        {
            if (!IsAvailable || !_isActuallyActive)
                return;

            try
            {
                await _arduino.Disconnect();
            }
            catch (Exception ex)
            {
                // log
                Status = ex.ToString();
            }

            _isActuallyActive = false;
        }

        public override async Task InitializeAsync()
        {
            _arduino = new ArduinoConnection(PinNumber, PositionOff, PositionIdle, PositionReady, PositionDepressed, PressTime);
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
