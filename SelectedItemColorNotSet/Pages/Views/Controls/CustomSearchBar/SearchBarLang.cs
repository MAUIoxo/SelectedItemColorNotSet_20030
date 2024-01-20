namespace SelectedItemColorNotSet.Pages.Views.Controls.CustomSearchBar
{
    public partial class SearchBarLang : SearchBar
    {
        public static readonly BindableProperty CancelButtonTextProperty = BindableProperty.Create(nameof(CancelButtonText), typeof(string), typeof(SearchBarLang), defaultValue: "Cancel");

        public string CancelButtonText
        {
            get => (string)GetValue(CancelButtonTextProperty);
            set => SetValue(CancelButtonTextProperty, value);
        }
    }
}
