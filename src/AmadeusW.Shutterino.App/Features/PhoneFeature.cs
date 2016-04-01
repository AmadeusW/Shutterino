using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Media;
using Windows.Phone.UI.Input;
using Windows.System.Display;
using Windows.UI.Core;

namespace AmadeusW.Shutterino.App.Features
{
    public class PhoneFeature : AFeature
    {
        // For listening to media property changes
        private readonly SystemMediaTransportControls _systemMediaControls = SystemMediaTransportControls.GetForCurrentView();

        // Prevents the screen from sleeping
        private readonly DisplayRequest _displayRequest = new DisplayRequest();

        public override async Task DeactivateAsync()
        {
            // There is nothing to deactivate
            return;
        }

        public override async Task ActivateAsync()
        {
            // There is nothing to activate
            return;
        }

        private void HardwareButtons_CameraPressed(object sender, CameraEventArgs e)
        {
            ShutterinoLogic.Instance.SuggestPhotoOpportunity(this);
        }

        public override async Task InitializeAsync()
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.CameraPressed += HardwareButtons_CameraPressed;
            }
            // Attempt to lock page to landscape orientation to prevent the CaptureElement from rotating, as this gives a better experience
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;

            // Hide the status bar
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }

            // Keep the screen always on
            _displayRequest.RequestActive();

            IsAvailable = true;
        }

        public override async Task CleanupAsync()
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.CameraPressed -= HardwareButtons_CameraPressed;
            }
            // Show the status bar
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
            }

            // Allow screen to turn off
            _displayRequest.RequestRelease();

            // Revert orientation preferences
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
        }
    }
}
