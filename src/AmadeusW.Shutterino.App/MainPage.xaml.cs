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
using AmadeusW.Shutterino.App.Features;
using Windows.UI.Core;

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
        private bool _initializedShapePosition;

        #region Constructor, lifecycle and navigation

        public MainPage()
        {
            this.InitializeComponent();
            _logic = ShutterinoLogic.Get(Dispatcher, PreviewControl, PhotoTakenCallback);

            // Do not cache the state of the UI when suspending/navigating
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // Useful to know when to initialize/clean up the camera
            Application.Current.Suspending += Application_Suspending;
            Application.Current.Resuming += Application_Resuming;
        }

        private void _timer_Tick(object sender, object e)
        {
            if (AccelerometerFeature.Instance.IsAvailable && AccelerometerFeature.Instance.IsActive)
                placeShapes();
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            // Handle global events no matter which page is active
            var deferral = e.SuspendingOperation.GetDeferral();
            await _logic.CleanUpAsync();
            deferral.Complete();
        }

        private async void Application_Resuming(object sender, object o)
        {
            // Handle global application events only if this page is active
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                await _logic.InitializeAsync();  // guaranteed to happen only once
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var logicTask = _logic.InitializeAsync(); // guaranteed to happen only once

            initializeCanvas();
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(20)
            };
            _timer.Tick += _timer_Tick;
            _timer.Start();

            await logicTask;
            await _logic.ActivateAsync();
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            _timer.Stop();
            _timer.Tick -= _timer_Tick;

            _logic.TakesPhotos = false;
            PhotoButton.IsChecked = false;

            await _logic.DeactivateAsync();
        }

        #endregion Constructor, lifecycle and navigation

        private async Task PhotoTakenCallback()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                PhotoNowButton.Background = highPrecisionBrush;
                await Task.Delay(200);
                PhotoNowButton.Background = Resources["TranslucentBlackBrush"] as SolidColorBrush;
            });
        }

        // TODO: use a viewmodel for all this:
        private void PhotoButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (PhotoButton.IsChecked.Value)
            {
                _logic.TakesPhotos = true;
            }
            else
            {
                _logic.TakesPhotos = false;
            }
        }

        private async void PhotoNowButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await _logic.SuggestPhotoOpportunity(null);
        }

        private void CalibrationButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _logic.Callibrate();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(SettingsPage));
        }

        private void initializeCanvas()
        {
            if (AccelerometerFeature.Instance.IsActive)
            {
                currentRoll.Visibility = Visibility.Visible;
                capturedRoll.Visibility = Visibility.Visible;
                targetRoll.Visibility = Visibility.Visible;
            }
            else
            {
                currentRoll.Visibility = Visibility.Collapsed;
                capturedRoll.Visibility = Visibility.Collapsed;
                targetRoll.Visibility = Visibility.Collapsed;
            }
        }

        private void placeShapes()
        {
            var canvasMiddle = new Point(visualization.ActualWidth / 2d, visualization.ActualHeight / 2d);
            var rollCanvasMiddle = new Point(currentRoll.ActualWidth / 2d, currentRoll.ActualHeight / 2d);

            if (!_initializedShapePosition)
            {
                if (visualization.ActualWidth != 0)
                {
                    Canvas.SetLeft(currentRoll, canvasMiddle.X - rollCanvasMiddle.X);
                    Canvas.SetTop(currentRoll, canvasMiddle.Y - rollCanvasMiddle.Y);
                    Canvas.SetLeft(capturedRoll, canvasMiddle.X - rollCanvasMiddle.X);
                    Canvas.SetTop(capturedRoll, canvasMiddle.Y - rollCanvasMiddle.Y);
                    Canvas.SetLeft(targetRoll, canvasMiddle.X - rollCanvasMiddle.X);
                    Canvas.SetTop(targetRoll, canvasMiddle.Y - rollCanvasMiddle.Y);
                    _initializedShapePosition = true;
                }
            }

            var scaleY = canvasMiddle.Y;
            var scaleX = canvasMiddle.X;

            var accelerometer = AccelerometerFeature.Instance;
            var shouldShowPreviousReading = accelerometer.RollOffset != 0 && accelerometer.PitchOffset != 0;
            capturedPitch.Visibility = capturedRoll.Visibility = shouldShowPreviousReading ? Visibility.Visible : Visibility.Collapsed;

            // Top-Bottom shows pitch (Z)
            Canvas.SetTop(currentPitch, canvasMiddle.Y + accelerometer.Pitch * scaleY);
            Canvas.SetTop(targetPitch, canvasMiddle.Y + accelerometer.TargetPitch * scaleY);
            if (shouldShowPreviousReading)
                Canvas.SetTop(capturedPitch, canvasMiddle.Y + accelerometer.CapturedPitch * scaleY);

            // Rotation shows roll (X)
            currentRoll.RenderTransform = new RotateTransform() { Angle = accelerometer.Roll * 180, CenterX = rollCanvasMiddle.X, CenterY = rollCanvasMiddle.Y };
            targetRoll.RenderTransform = new RotateTransform() { Angle = accelerometer.TargetRoll * 180, CenterX = rollCanvasMiddle.X, CenterY = rollCanvasMiddle.Y };
            if (shouldShowPreviousReading)
                capturedRoll.RenderTransform = new RotateTransform() { Angle = accelerometer.CapturedRoll * 180, CenterX = rollCanvasMiddle.X, CenterY = rollCanvasMiddle.Y };

            currentPitch.Stroke =
                accelerometer.DeltaPitch < AccelerometerFeature.Instance.Precision ? highPrecisionBrush
                : accelerometer.DeltaPitch < AccelerometerFeature.Instance.Precision * 2 ? lowPrecisionBrush
                : accelerometer.DeltaPitch < AccelerometerFeature.Instance.Precision * 4 ? hintPrecisionBrush
                : noPrecisionBrush;
            currentRollRectangle.Stroke =
                accelerometer.DeltaRoll < AccelerometerFeature.Instance.Precision ? highPrecisionBrush
                : accelerometer.DeltaRoll < AccelerometerFeature.Instance.Precision * 2 ? lowPrecisionBrush
                : accelerometer.DeltaRoll < AccelerometerFeature.Instance.Precision * 4 ? hintPrecisionBrush
                : noPrecisionBrush;
        }

    }
}
