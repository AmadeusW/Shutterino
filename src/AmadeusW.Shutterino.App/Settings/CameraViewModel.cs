using AmadeusW.Shutterino.App.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App.Settings
{
    public class CameraViewModel : ShutterinoModuleViewModel
    {
        CameraFeature _camera => Device as CameraFeature;

        public int PhotoCount
        {
            get { return _photoCount; }
            set
            {
                if (_photoCount != value)
                {
                    _photoCount = value;
                    NotifyPropertyChanged();
                    _camera.PhotoCount = _photoCount;
                }
            }
        }

        public CameraViewModel() : base(CameraFeature.Instance)
        {
            PhotoCount = _camera.PhotoCount;
        }

        #region Backing Fields

        int _photoCount;

        #endregion

    }
}
