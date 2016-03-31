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
using AmadeusW.Shutterino.App.Devices;

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
            new ShutterinoSettingDefinition() { Name="Accelerometer", Icon="Trim", ViewType=typeof(AccelerometerView), DeviceViewModel=new AccelerometerViewModel()},
            new ShutterinoSettingDefinition() { Name="Arduino", Icon="Target", ViewType=typeof(ArduinoView), DeviceViewModel=new ArduinoViewModel()},
            new ShutterinoSettingDefinition() { Name="Camera", Icon="Camera", ViewType=typeof(CameraView), DeviceViewModel=new CameraViewModel()},
            new ShutterinoSettingDefinition() { Name="Edge detector", Icon="Contact2", ViewType=typeof(EdgeDetectorView), DeviceViewModel=new EdgeDetectorViewModel()},
            new ShutterinoSettingDefinition() { Name="Location", Icon="Map", ViewType=typeof(LocationView), DeviceViewModel=new LocationViewModel()},
            new ShutterinoSettingDefinition() { Name="Log", Icon="List", ViewType=typeof(LogView), DeviceViewModel=new LogViewModel()},
            new ShutterinoSettingDefinition() { Name="Timer", Icon="Clock", ViewType=typeof(TimerView), DeviceViewModel=new TimerViewModel()},
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
            if (this.Frame.CanGoBack) this.Frame.GoBack();
            e.Handled = true;
        }

        private void SettingsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox settingsListBox = sender as ListBox;
            var s = settingsListBox.SelectedItem as ShutterinoSettingDefinition;
            if (s != null)
            {
                SettingsFrame.Navigate(s.ViewType);
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }

    /// <summary>
    /// Scenario Class Object
    /// </summary>
    public class ShutterinoSettingDefinition
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool Active { get; set; }
        public Type ViewType { get; set; }
        public ShutterinoModuleViewModel DeviceViewModel { get; set; }

        public override string ToString() => $"{Name} settings page";
    }
}
