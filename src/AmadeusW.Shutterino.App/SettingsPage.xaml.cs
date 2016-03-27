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
            new ShutterinoSettingDefinition() { Name="Accelerometer", Icon="Trim", ViewType=typeof(AccelerometerView)},
            new ShutterinoSettingDefinition() { Name="Arduino", Icon="Target", ViewType=typeof(ArduinoView)},
            new ShutterinoSettingDefinition() { Name="Camera", Icon="Camera", ViewType=typeof(CameraView)},
            new ShutterinoSettingDefinition() { Name="Edge detector", Icon="Contact2", ViewType=typeof(EdgeDetectorView)},
            new ShutterinoSettingDefinition() { Name="Location", Icon="Map", ViewType=typeof(LocationView)},
            new ShutterinoSettingDefinition() { Name="Log", Icon="List", ViewType=typeof(LogView)},
            new ShutterinoSettingDefinition() { Name="Timer", Icon="Clock", ViewType=typeof(TimerView)},
        };

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SettingsControl.ItemsSource = _settings;
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
    }

    /// <summary>
    /// Scenario Class Object
    /// </summary>
    public class ShutterinoSettingDefinition
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public Type ViewType { get; set; }

        public override string ToString() => $"{Name} settings page";
    }
}
