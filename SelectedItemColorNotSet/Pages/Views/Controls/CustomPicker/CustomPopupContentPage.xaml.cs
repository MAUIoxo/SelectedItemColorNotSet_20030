using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace SelectedItemColorNotSet.Pages.Views.Controls.CustomPicker;

public partial class CustomPopup : Popup
{
    public CustomPopup()
    {
        InitializeComponent();
    }

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        Close();
        //WeakReferenceMessenger.Default.Send(new UnregisterCustomPickerPopup(true));
    }

    /// <summary>
    /// Gets called after finishing the OK-Button-Command and will then Close this thing
    /// and unregister the CustomPickerPopup so that it does not crash when rotating orientations
    /// after it was closed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OkButton_Clicked(object sender, EventArgs e)
    {
        Close();
        //WeakReferenceMessenger.Default.Send(new UnregisterCustomPickerPopup(true));
    }

    private void PopupParentGrid_SizeChanged(object sender, EventArgs e)
    {
        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            PopupCollectionView.HeightRequest = PopupParentGrid.Height - 175;
        }
    }
}