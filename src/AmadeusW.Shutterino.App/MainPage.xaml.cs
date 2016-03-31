﻿using System;
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
using AmadeusW.Shutterino.App.Devices;

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
            _logic = ShutterinoLogic.Get(Dispatcher, PreviewControl);

            // Do not cache the state of the UI when suspending/navigating
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // Useful to know when to initialize/clean up the camera
            Application.Current.Suspending += Application_Suspending;
            Application.Current.Resuming += Application_Resuming;
        }

        private void _timer_Tick(object sender, object e)
        {
            if (Devices.DAccelerometer.Instance.IsAvailable)
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

            _logic.EndTakingPhotos();
            PhotoButton.IsChecked = false;

            await _logic.DeactivateAsync();
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

        /*
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
        }*/

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(SettingsPage));
        }

        private void initializeCanvas()
        {
            var canvasMiddle = new Point(visualization.ActualWidth / 2, visualization.ActualHeight / 2);
            var rollCanvasMiddle = new Point(currentRoll.ActualWidth / 2, currentRoll.ActualHeight / 2);

            Canvas.SetLeft(currentRoll, canvasMiddle.X - rollCanvasMiddle.X);
            Canvas.SetTop(currentRoll, canvasMiddle.Y - rollCanvasMiddle.Y);
            Canvas.SetLeft(capturedRoll, canvasMiddle.X - rollCanvasMiddle.X);
            Canvas.SetTop(capturedRoll, canvasMiddle.Y - rollCanvasMiddle.Y);
            Canvas.SetLeft(targetRoll, canvasMiddle.X - rollCanvasMiddle.X);
            Canvas.SetTop(targetRoll, canvasMiddle.Y - rollCanvasMiddle.Y);
        }

        private void placeShapes()
        {
            var canvasMiddle = new Point(visualization.ActualWidth / 2, visualization.ActualHeight / 2);
            var rollCanvasMiddle = new Point(currentRoll.ActualWidth / 2, currentRoll.ActualHeight / 2);

            var scaleY = canvasMiddle.Y;
            var scaleX = canvasMiddle.X;

            var accelerometer = Devices.DAccelerometer.Instance;
            var shouldShowPreviousReading = accelerometer.RollOffset != 0 && accelerometer.PitchOffset != 0;
            capturedPitch.Visibility = capturedRoll.Visibility = shouldShowPreviousReading ? Visibility.Visible : Visibility.Collapsed;

            // Top-Bottom shows pitch (Z)
            Canvas.SetTop(currentPitch, canvasMiddle.Y + accelerometer.Pitch * scaleY);
            Canvas.SetTop(targetPitch, canvasMiddle.Y + accelerometer.TargetPitch * scaleY);
            if (shouldShowPreviousReading)
                Canvas.SetTop(capturedPitch, canvasMiddle.Y + accelerometer.CapturedPitch * scaleY);

            // Rotation shows roll (X)
            currentRoll.RenderTransform = new RotateTransform() { Angle = accelerometer.Roll * 180, CenterX = rollCanvasMiddle.X, CenterY = rollCanvasMiddle.Y };
            targetRoll.RenderTransform = new RotateTransform() { Angle = accelerometer.CapturedRoll * 180, CenterX = rollCanvasMiddle.X, CenterY = rollCanvasMiddle.Y };
            if (shouldShowPreviousReading)
                capturedRoll.RenderTransform = new RotateTransform() { Angle = accelerometer.CapturedRoll * 180, CenterX = rollCanvasMiddle.X, CenterY = rollCanvasMiddle.Y };

            currentPitch.Stroke =
                accelerometer.DeltaPitch < DAccelerometer.HIGH_PRECISION ? highPrecisionBrush
                : accelerometer.DeltaPitch < DAccelerometer.LOW_PRECISION ? lowPrecisionBrush
                : accelerometer.DeltaPitch < DAccelerometer.HINT_PRECISION ? hintPrecisionBrush
                : noPrecisionBrush;
            currentRollRectangle.Stroke =
                accelerometer.DeltaRoll < DAccelerometer.HIGH_PRECISION ? highPrecisionBrush
                : accelerometer.DeltaRoll < DAccelerometer.LOW_PRECISION ? lowPrecisionBrush
                : accelerometer.DeltaRoll < DAccelerometer.HINT_PRECISION ? hintPrecisionBrush
                : noPrecisionBrush;
        }
    }
}
