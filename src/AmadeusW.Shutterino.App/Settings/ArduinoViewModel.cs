using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Settings
{
    public class ArduinoViewModel : ShutterinoModuleViewModel
    {
        /// <summary>
        /// Connection setting
        /// </summary>
        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Connection setting
        /// </summary>
        public string PortNumber
        {
            get { return _portNumber; }
            set { _portNumber = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Servo setting
        /// </summary>
        public byte PositionOff
        {
            get { return _positionOff; }
            set { _positionOff = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Servo setting
        /// </summary>
        public byte PositionIdle
        {
            get { return _positionIdle; }
            set { _positionIdle = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Servo setting
        /// </summary>
        public byte PositionReady
        {
            get { return _positionReady; }
            set { _positionReady = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Servo setting
        /// </summary>
        public byte PositionPressed
        {
            get { return _positionPressed; }
            set { _positionPressed = value; NotifyPropertyChanged(); }
        }

        #region Backing Fields

        private string _portNumber;
        private string _hostName;
        private byte _positionOff;
        private byte _positionIdle;
        private byte _positionReady;
        private byte _positionPressed;

        #endregion

    }
}
