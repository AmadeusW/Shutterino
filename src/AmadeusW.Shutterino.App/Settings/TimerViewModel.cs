using AmadeusW.Shutterino.App.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Settings
{
    public class TimerViewModel : ShutterinoModuleViewModel
    {
        TimerFeature _timer => Device as TimerFeature;

        public TimerViewModel() : base(TimerFeature.Instance)
        {
            Delay1 = (int)_timer.Delay1.TotalSeconds;
            Delay2 = (int)_timer.Delay2.TotalSeconds;
            Delay3 = (int)_timer.Delay3.TotalSeconds;
            Delay4 = (int)_timer.Delay4.TotalSeconds;
            Delay2Active = _timer.Delay2Active;
            Delay3Active = _timer.Delay3Active;
            Delay4Active = _timer.Delay4Active;
        }

        public int Delay1
        {
            get { return _delay1; }
            set
            {
                if (_delay1 != value)
                {
                    _delay1 = value;
                    NotifyPropertyChanged();
                    _timer.Delay1 = TimeSpan.FromSeconds(_delay1);
                }
            }
        }

        public int Delay2
        {
            get { return _delay2; }
            set
            {
                if (_delay2 != value)
                {
                    _delay2 = value;
                    NotifyPropertyChanged();
                    _timer.Delay2 = TimeSpan.FromSeconds(_delay2);
                }
            }
        }

        public int Delay3
        {
            get { return _delay3; }
            set
            {
                if (_delay3 != value)
                {
                    _delay3 = value;
                    NotifyPropertyChanged();
                    _timer.Delay3 = TimeSpan.FromSeconds(_delay3);
                }
            }
        }

        public int Delay4
        {
            get { return _delay4; }
            set
            {
                if (_delay4 != value)
                {
                    _delay4 = value;
                    NotifyPropertyChanged();
                    _timer.Delay4 = TimeSpan.FromSeconds(_delay4);
                }
            }
        }

        public bool Delay2Active
        {
            get { return _delay2Active; }
            set
            {
                if (_delay2Active != value)
                {
                    _delay2Active = value;
                    NotifyPropertyChanged();
                    _timer.Delay2Active = _delay2Active;
                }
            }
        }

        public bool Delay3Active
        {
            get { return _delay3Active; }
            set
            {
                if (_delay3Active != value)
                {
                    _delay3Active = value;
                    NotifyPropertyChanged();
                    _timer.Delay3Active = _delay3Active;
                }
            }
        }

        public bool Delay4Active
        {
            get { return _delay4Active; }
            set
            {
                if (_delay4Active != value)
                {
                    _delay4Active = value;
                    NotifyPropertyChanged();
                    _timer.Delay4Active = _delay4Active;
                }
            }
        }


        #region Backing Fields

        private int _delay1;
        private int _delay2;
        private int _delay3;
        private int _delay4;
        private bool _delay2Active;
        private bool _delay3Active;
        private bool _delay4Active;

        #endregion

    }
}
