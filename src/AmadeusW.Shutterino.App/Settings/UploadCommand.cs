using AmadeusW.Shutterino.App.Features;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AmadeusW.Shutterino.App.Settings
{
    internal class UploadCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Task.Run(async () =>
            {
                try
                {
                    await LogFeature.Instance.Upload();
                }
                catch (Exception ex)
                {
                    // TODO: log
                }
            });
        }
    }
}