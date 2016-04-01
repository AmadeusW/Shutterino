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
        private ShutterinoModuleViewModel _vm;

        public ToggleCommand(ShutterinoModuleViewModel vm)
        {
            _vm = vm;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _vm.Available;
        }

        public async void Execute(object parameter)
        {
            try
            {
                _vm.Active = !_vm.Active;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception caught in {nameof(ToggleCommand)}: {ex}");
            }
        }
    }
}