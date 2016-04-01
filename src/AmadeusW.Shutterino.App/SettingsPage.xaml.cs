using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AmadeusW.Shutterino.App.Settings;
using Windows.UI.Core;
using AmadeusW.Shutterino.App.Features;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AmadeusW.Shutterino.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        List<ShutterinoSettingDefinition> _settings = new List<ShutterinoSettingDefinition>
        {
            new ShutterinoSettingDefinition() { Name="Accelerometer", Icon="Trim", ViewType=typeof(AccelerometerView), DeviceViewModel=Application.Current.Resources["AccelerometerViewModel"] as ShutterinoModuleViewModel},
            new ShutterinoSettingDefinition() { Name="Arduino", Icon="Target", ViewType=typeof(ArduinoView), DeviceViewModel=Application.Current.Resources["ArduinoViewModel"] as ShutterinoModuleViewModel},
            new ShutterinoSettingDefinition() { Name="Camera", Icon="Camera", ViewType=typeof(CameraView), DeviceViewModel=Application.Current.Resources["CameraViewModel"] as ShutterinoModuleViewModel},
            new ShutterinoSettingDefinition() { Name="Edge detector", Icon="Contact2", ViewType=typeof(EdgeDetectorView), DeviceViewModel=Application.Current.Resources["EdgeDetectorViewModel"] as ShutterinoModuleViewModel},
            new ShutterinoSettingDefinition() { Name="Location", Icon="Map", ViewType=typeof(LocationView), DeviceViewModel=Application.Current.Resources["LocationViewModel"] as ShutterinoModuleViewModel},
            new ShutterinoSettingDefinition() { Name="Log", Icon="List", ViewType=typeof(LogView), DeviceViewModel=Application.Current.Resources["LogViewModel"] as ShutterinoModuleViewModel},
            new ShutterinoSettingDefinition() { Name="Timer", Icon="Clock", ViewType=typeof(TimerView), DeviceViewModel=Application.Current.Resources["TimerViewModel"] as ShutterinoModuleViewModel},
        };

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsControl.ItemsSource = _settings;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequestedHandler;

            base.OnNavigatedTo(e);
        }

        private void BackRequestedHandler(object sender, BackRequestedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                e.Handled = true;
            }
        }

        private void SettingsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox settingsListBox = sender as ListBox;
            var s = settingsListBox.SelectedItem as ShutterinoSettingDefinition;
            if (s?.DeviceViewModel?.Available == true)
            {
                SettingsFrame.Navigate(s.ViewType);
                // TODO: this should be properly done with binding
                StatusBlock.Text = s.DeviceViewModel.Status ?? String.Empty;
            }
        }
    }

    /// <summary>
    /// Scenario Class Object
    /// </summary>
    public class ShutterinoSettingDefinition
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public Type ViewType { get; set; }
        public ShutterinoModuleViewModel DeviceViewModel { get; set; }

        public override string ToString() => $"{Name} settings page";
    }
}
