﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace AmadeusW.Shutterino.App.Features
{
    public class TimerFeature : AFeature
    {
        public static TimerFeature Instance { get; private set; }
        public override string ToString() => "Timer";

        public TimeSpan Delay1 { get; set; }
        public TimeSpan Delay2 { get; set; }
        public TimeSpan Delay3 { get; set; }
        public TimeSpan Delay4 { get; set; }
        public bool Delay2Active { get; set; }
        public bool Delay3Active { get; set; }
        public bool Delay4Active { get; set; }

        private int _currentDelayId = 1;

        private DispatcherTimer _photoTakingTimer;

        public TimerFeature() : base()
        {
            Instance = this;
            IsAvailable = true;

            Delay1 = (TimeSpan)(_localSettings.Values["timer-Delay1"] ?? TimeSpan.FromSeconds(9));
            Delay2 = (TimeSpan)(_localSettings.Values["timer-Delay2"] ?? TimeSpan.FromSeconds(1));
            Delay3 = (TimeSpan)(_localSettings.Values["timer-Delay3"] ?? TimeSpan.FromSeconds(0));
            Delay4 = (TimeSpan)(_localSettings.Values["timer-Delay4"] ?? TimeSpan.FromSeconds(0));
            Delay2Active = (bool)(_localSettings.Values["timer-Delay2Active"] ?? true);
            Delay3Active = (bool)(_localSettings.Values["timer-Delay3Active"] ?? false);
            Delay4Active = (bool)(_localSettings.Values["timer-Delay4Active"] ?? false);

            IsActive = (bool)(_localSettings.Values["timer-IsActive"] ?? false);
        }

        protected override async Task ActivateAsyncCore()
        {
            if (!IsAvailable || _isActuallyActive)
                return;

            // Activate only if user wants to
            if (IsActive)
            {
                _currentDelayId = 1;
                _photoTakingTimer.Interval = Delay1;
                _photoTakingTimer.Start();

                _isActuallyActive = true;
            }
        }

        protected override async Task CleanupAsyncCore()
        {
            _photoTakingTimer.Tick -= _photoTakingTimer_Tick;

            _localSettings.Values["timer-Delay1"] = Delay1;
            _localSettings.Values["timer-Delay2"] = Delay2;
            _localSettings.Values["timer-Delay3"] = Delay3;
            _localSettings.Values["timer-Delay4"] = Delay4;
            _localSettings.Values["timer-Delay2Active"] = Delay2Active;
            _localSettings.Values["timer-Delay3Active"] = Delay3Active;
            _localSettings.Values["timer-Delay4Active"] = Delay4Active;

            _localSettings.Values["timer-IsActive"] = IsActive;
        }

        protected override async Task DeactivateAsyncCore()
        {
            if (!IsAvailable || !_isActuallyActive)
                return;

            _photoTakingTimer.Stop();

            _isActuallyActive = false;
        }

        protected override async Task InitializeAsyncCore()
        {
            _photoTakingTimer = new DispatcherTimer();
            _photoTakingTimer.Tick += _photoTakingTimer_Tick;
        }

        private void _photoTakingTimer_Tick(object sender, object e)
        {
            if (ShutterinoLogic.Instance.TakesPhotos)
            {
                Task.Run(async () => await ShutterinoLogic.Instance.SuggestPhotoOpportunity(this));
            }

            switch (_currentDelayId)
            {
                case 1:
                    if (Delay2Active && Delay2.TotalSeconds > 0)
                    {
                        _photoTakingTimer.Interval = Delay2;
                        _currentDelayId = 2;
                    }
                    else
                    {
                        _photoTakingTimer.Interval = Delay1;
                        _currentDelayId = 1;
                    }
                    break;
                case 2:
                    if (Delay3Active && Delay3.TotalSeconds > 0)
                    {
                        _photoTakingTimer.Interval = Delay3;
                        _currentDelayId = 3;
                    }
                    else
                    {
                        _photoTakingTimer.Interval = Delay1;
                        _currentDelayId = 1;
                    }
                    break;
                case 3:
                    if (Delay4Active && Delay4.TotalSeconds > 0)
                    {
                        _photoTakingTimer.Interval = Delay4;
                        _currentDelayId = 4;
                    }
                    else
                    {
                        _photoTakingTimer.Interval = Delay1;
                        _currentDelayId = 1;
                    }
                    break;
                case 4:
                    _photoTakingTimer.Interval = Delay1;
                    _currentDelayId = 1;
                    break;
            }
        }
    }
}
