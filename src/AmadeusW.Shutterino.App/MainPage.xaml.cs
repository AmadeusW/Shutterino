using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Media;
using Windows.Phone.UI.Input;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Sensors;
using Windows.Media.Capture;
using Windows.UI.Xaml.Shapes;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AmadeusW.Shutterino.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ShutterinoLogic _logic;
        DispatcherTimer _timer;

        SolidColorBrush highPrecisionBrush = new SolidColorBrush(Colors.LimeGreen);
        SolidColorBrush lowPrecisionBrush = new SolidColorBrush(Colors.Yellow);
        SolidColorBrush hintPrecisionBrush = new SolidColorBrush(Colors.Orange);
        SolidColorBrush noPrecisionBrush = new SolidColorBrush(Colors.Red);

        #region Constructor, lifecycle and navigation

        public MainPage()
        {
            this.InitializeComponent();
            _logic = new ShutterinoLogic(Dispatcher, PreviewControl);

            // Do not cache the state of the UI when suspending/navigating
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // Useful to know when to initialize/clean up the camera
            Application.Current.Suspending += Application_Suspending;
            Application.Current.Resuming += Application_Resuming;

            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(20)
            };
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object sender, object e)
        {
            placeShapes();
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            // Handle global application events only if this page is active
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await _logic.CleanUpAsync();
                deferral.Complete();
            }
        }

        private async void Application_Resuming(object sender, object o)
        {
            // Handle global application events only if this page is active
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                await _logic.InitializeAsync();
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await _logic.InitializeAsync();
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            await _logic.CleanUpAsync();
        }

        #endregion Constructor, lifecycle and navigation

        // TODO: use a viewmodel for all this:
        private async void PhotoButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PhotoButton.IsChecked.Value)
            {
                await _logic.TakePhoto();
                _logic.BeginTakingPhotos();
            }
            else
            {
                _logic.EndTakingPhotos();
            }
        }

        private void PrecisionButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PrecisionButton.IsChecked.Value)
            {
                _logic.UseHighPrecision();
            }
            else
            {
                _logic.UseLowPrecision();
            }
        }

        private void CalibrationButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _logic.Callibrate();
        }

        private async void ArduinoButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ArduinoButton.IsChecked.Value)
                {
                    var status = await _logic.initializeArduino(4, 30, 90, 10, "192.168.137.164", 3030);
                    System.Diagnostics.Debug.WriteLine("Connection: " + status.ToString());
                }
                else
                {
                    var status = await _logic.disableArduino();
                    System.Diagnostics.Debug.WriteLine("Disconnecting: " + status.ToString());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                System.Diagnostics.Debugger.Break();
            }
        }

        private void placeShapes()
        {
            var canvasMiddle = new Point(visualization.ActualWidth / 2, visualization.ActualHeight / 2);
            var rollCanvasMiddle = new Point(currentRoll.ActualWidth / 2, currentRoll.ActualHeight / 2);

            var scaleY = canvasMiddle.Y;
            var scaleX = canvasMiddle.X;

            // Left-Right shows yaw (Y)
            Canvas.SetLeft(currentYaw, canvasMiddle.X + Devices.DAccelerometer.Instance.Yaw * scaleX);
            Canvas.SetLeft(capturedYaw, canvasMiddle.X + Devices.DAccelerometer.Instance.CapturedYaw * scaleX);
            // Top-Bottom shows pitch (Z)
            Canvas.SetTop(currentPitch, canvasMiddle.Y + Devices.DAccelerometer.Instance.Pitch * scaleY);
            Canvas.SetTop(capturedPitch, canvasMiddle.Y + Devices.DAccelerometer.Instance.CapturedPitch * scaleY);
            // Rotation shows roll (X)
            Canvas.SetLeft(currentRoll, canvasMiddle.X - rollCanvasMiddle.X);
            Canvas.SetTop(currentRoll, canvasMiddle.Y - rollCanvasMiddle.Y);
            Canvas.SetLeft(capturedRoll, canvasMiddle.X - rollCanvasMiddle.X);
            Canvas.SetTop(capturedRoll, canvasMiddle.Y - rollCanvasMiddle.Y);
            currentRoll.RenderTransform = new RotateTransform() { Angle = Devices.DAccelerometer.Instance.Roll * 180, CenterX = rollCanvasMiddle.X, CenterY = rollCanvasMiddle.Y };
            capturedRoll.RenderTransform = new RotateTransform() { Angle = Devices.DAccelerometer.Instance.CapturedRoll * 180, CenterX = rollCanvasMiddle.X, CenterY = rollCanvasMiddle.Y };

            currentYaw.Stroke =
                Devices.DAccelerometer.Instance.DeltaYaw < ShutterinoLogic.HIGH_PRECISION ? highPrecisionBrush
                : Devices.DAccelerometer.Instance.DeltaYaw < ShutterinoLogic.LOW_PRECISION ? lowPrecisionBrush
                : Devices.DAccelerometer.Instance.DeltaYaw < ShutterinoLogic.HINT_PRECISION ? hintPrecisionBrush
                : noPrecisionBrush;
            currentPitch.Stroke =
                Devices.DAccelerometer.Instance.DeltaPitch < ShutterinoLogic.HIGH_PRECISION ? highPrecisionBrush
                : Devices.DAccelerometer.Instance.DeltaPitch < ShutterinoLogic.LOW_PRECISION ? lowPrecisionBrush
                : Devices.DAccelerometer.Instance.DeltaPitch < ShutterinoLogic.HINT_PRECISION ? hintPrecisionBrush
                : noPrecisionBrush;
            currentRollRectangle.Stroke =
                Devices.DAccelerometer.Instance.DeltaRoll < ShutterinoLogic.HIGH_PRECISION ? highPrecisionBrush
                : Devices.DAccelerometer.Instance.DeltaRoll < ShutterinoLogic.LOW_PRECISION ? lowPrecisionBrush
                : Devices.DAccelerometer.Instance.DeltaRoll < ShutterinoLogic.HINT_PRECISION ? hintPrecisionBrush
                : noPrecisionBrush;
        }
    }
}
