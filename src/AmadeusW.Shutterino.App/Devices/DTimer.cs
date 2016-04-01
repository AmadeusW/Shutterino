using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace AmadeusW.Shutterino.App.Devices
{
    public class DTimer : Device
    {
        public static DTimer Instance { get; private set; }
        public override string ToString() => "Timer";

        public TimeSpan Delay1 { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan Delay2 { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan Delay3 { get; set; } = TimeSpan.FromSeconds(0);
        public TimeSpan Delay4 { get; set; } = TimeSpan.FromSeconds(0);
        public bool Delay2Active { get; set; } = true;
        public bool Delay3Active { get; set; } = false;
        public bool Delay4Active { get; set; } = false;

        private int _currentDelayId = 1;

        private DispatcherTimer _photoTakingTimer;

        public DTimer() : base()
        {
            Instance = this;
            IsAvailable = true;
        }

        public override async Task ActivateAsync()
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

        public override async Task CleanupAsync()
        {
            _photoTakingTimer.Tick -= _photoTakingTimer_Tick;
        }

        public override async Task DeactivateAsync()
        {
            if (!IsAvailable || !_isActuallyActive)
                return;

            _photoTakingTimer.Stop();

            _isActuallyActive = false;
        }

        public override async Task InitializeAsync()
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
                    if (Delay2Active)
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
                    if (Delay3Active)
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
                    if (Delay4Active)
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
