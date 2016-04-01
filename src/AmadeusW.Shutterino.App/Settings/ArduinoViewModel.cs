using AmadeusW.Shutterino.App.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Settings
{
    public class ArduinoViewModel : ShutterinoModuleViewModel
    {
        DArduino _arduino => _device as DArduino;

        public ArduinoViewModel() : base(DArduino.Instance)
        {
            HostName = _arduino.HostName;
            PortNumber = _arduino.PortNumber;
            PositionOff = _arduino.PositionOff;
            PositionIdle = _arduino.PositionIdle;
            PositionReady = _arduino.PositionReady;
            PositionDepressed = _arduino.PositionDepressed;
        }

        /// <summary>
        /// Connection setting
        /// </summary>
        public string HostName
        {
            get { return _hostName; }
            set
            {
                if (_hostName != value)
                {
                    _hostName = value;
                    NotifyPropertyChanged();
                    _arduino.HostName = _hostName;
                }
            }
        }

        /// <summary>
        /// Connection setting
        /// </summary>
        public string PortNumber
        {
            get { return _portNumber; }
            set
            {
                if (_hostName != value)
                {
                    _hostName = value;
                    NotifyPropertyChanged();
                    _arduino.HostName = _hostName;
                }
            }
        }

        /// <summary>
        /// Servo setting
        /// </summary>
        public byte PositionOff
        {
            get { return _positionOff; }
            set
            {
                if (_positionOff != value)
                {
                    _positionOff = value;
                    NotifyPropertyChanged();
                    _arduino.PositionOff = _positionOff;
                }
            }
        }

        /// <summary>
        /// Servo setting
        /// </summary>
        public byte PositionIdle
        {
            get { return _positionIdle; }
            set
            {
                if (_positionIdle != value)
                {
                    _positionIdle = value;
                    NotifyPropertyChanged();
                    _arduino.PositionIdle = _positionIdle;
                }
            }
        }

        /// <summary>
        /// Servo setting
        /// </summary>
        public byte PositionReady
        {
            get { return _positionReady; }
            set
            {
                if (_positionReady != value)
                {
                    _hostName = value;
                    NotifyPropertyChanged();
                    _arduino.PositionReady = _positionReady;
                }
            }
        }

        /// <summary>
        /// Servo setting
        /// </summary>
        public byte PositionDepressed
        {
            get { return _positionDepressed; }
            set
            {
                if (_positionDepressed != value)
                {
                    _positionDepressed = value;
                    NotifyPropertyChanged();
                    _arduino.PositionDepressed = _positionDepressed;
                }
            }
        }

        #region Backing Fields

        private string _portNumber;
        private string _hostName;
        private byte _positionOff;
        private byte _positionIdle;
        private byte _positionReady;
        private byte _positionDepressed;

        #endregion

    }
}
