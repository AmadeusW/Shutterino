using AmadeusW.Shutterino.App.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmadeusW.Shutterino.App.Settings
{
    public class LogViewModel : ShutterinoModuleViewModel
    {
        LogFeature _logger => Device as LogFeature;

        public string LastSync => _logger.LastSync;
        public int PhotoCount => _logger.PhotoCount;
        public ICommand UploadCommand { get; }

        public LogViewModel() : base(LogFeature.Instance)
        {
            UploadCommand = new UploadCommand();
        }

        #region Backing Fields



        #endregion

    }
}
