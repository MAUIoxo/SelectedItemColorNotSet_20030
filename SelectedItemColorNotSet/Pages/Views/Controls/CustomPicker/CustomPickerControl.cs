namespace SelectedItemColorNotSet.Pages.Views.Controls.CustomPicker
{
    using CommunityToolkit.Maui.Views;
    using Microsoft.Maui.Controls;
    using Microsoft.Maui.Devices;
    using SelectedItemColorNotSet.Pages.Views.Controls.CustomEntry;
    using SelectedItemColorNotSet.ViewModels;

    public class CustomPickerControl : ContentView
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomPickerControl), string.Empty);
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(CustomPickerControl), string.Empty);
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(CustomPickerControl), Colors.Black);
        public static readonly BindableProperty BorderMarginProperty = BindableProperty.Create(nameof(BorderMargin), typeof(Thickness), typeof(CustomPickerControl), new Thickness(0, -5, 0, 0));
        public static new readonly BindableProperty MarginProperty = BindableProperty.Create(nameof(Margin), typeof(Thickness), typeof(CustomPickerControl), new Thickness(0, 0, 0, 0));


        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public Thickness BorderMargin
        {
            get => (Thickness)GetValue(BorderMarginProperty);
            set => SetValue(BorderMarginProperty, value);
        }

        public new Thickness Margin
        {
            get => (Thickness)GetValue(MarginProperty);
            set => SetValue(MarginProperty, value);
        }



        private readonly CustomPopupViewModel customPopupViewModel;
        private Popup customPickerPopup;

        public CustomPickerControl()
        {
            customPopupViewModel = new CustomPopupViewModel();

            customPickerPopup = new CustomPopup();
            customPickerPopup.BindingContext = customPopupViewModel;

            var entry = new BorderlessEntry()
            {
                ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
                PlaceholderColor = new Entry().PlaceholderColor
            };

            entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), source: this));
            entry.SetBinding(Entry.PlaceholderProperty, new Binding(nameof(Placeholder), source: this));
            entry.SetBinding(Entry.MarginProperty, new Binding(nameof(Margin), source: this));


            var grid = new Grid()
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                },

                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                },
            };
            grid.Add(entry, 0, 0);

            var imageButton = new ImageButton
            {
                Source = "chevron_down_solid.png",
                HeightRequest = 11,
                WidthRequest = 11,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(5, 0, 5, 0)
            };

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                imageButton.Aspect = Aspect.AspectFit;
                imageButton.Scale = 0.35;
            }

            imageButton.Clicked += OnDropDownButtonClicked;

            grid.Add(imageButton, 1, 0);

            var boxView = new BoxView
            {
                HeightRequest = 0.5,
            };

            boxView.SetBinding(BoxView.ColorProperty, new Binding(nameof(BorderColor), source: this));
            boxView.SetBinding(BoxView.MarginProperty, new Binding(nameof(BorderMargin), source: this));

            Grid.SetRow(boxView, 1);
            Grid.SetColumn(boxView, 0);
            Grid.SetColumnSpan(boxView, 2);
            grid.Children.Add(boxView);

            Content = grid;            
        }

        private void OnDropDownButtonClicked(object sender, EventArgs e)
        {
            // Set selected store correctly
            customPopupViewModel.SelectedSavedStoreName = Text;

            // Create new instance of the Popup to create it
            // correctly again after the it was closed before
            customPickerPopup = new CustomPopup();
            customPickerPopup.BindingContext = customPopupViewModel;

            // Adapt size of the Popup based on the current orientation
            AdaptPopupSizeBasedOnOrientation();


            var mainPage = Application.Current.MainPage;
            mainPage.ShowPopupAsync(customPickerPopup);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            AdaptPopupSizeBasedOnOrientation();
        }

        private void AdaptPopupSizeBasedOnOrientation()
        {
            if (customPickerPopup == null)
            {
                return;
            }
            
            if (DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Portrait)       // Portrait
            {
                customPickerPopup.Size = new Size(DeviceDisplay.MainDisplayInfo.Width * 0.3, DeviceDisplay.MainDisplayInfo.Height * 0.2);
            }
            else                                                                                        // Landscape
            {
                customPickerPopup.Size = new Size(DeviceDisplay.MainDisplayInfo.Width * 0.3, DeviceDisplay.MainDisplayInfo.Height * 0.3);
            }
        }
    }
}
