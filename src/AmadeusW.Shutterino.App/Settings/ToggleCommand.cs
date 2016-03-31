using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AmadeusW.Shutterino.App.Devices;

namespace AmadeusW.Shutterino.App.Settings
{
    public class ToggleCommand : ICommand
    {
        private Device _device;

        public ToggleCommand(Device device)
        {
            _device = device;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _device.IsAvailable;
        }

        public async void Execute(object parameter)
        {
            try
            {
                if (_device.IsActive)
                    await _device.DeactivateAsync();
                else
                    await _device.ActivateAsync();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception caught in {nameof(ToggleCommand)}: {ex}");
            }
        }
    }
}
