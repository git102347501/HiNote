using HiNote.Helpers;
using HiNote.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Windows.Globalization;

namespace HiNote.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        ViewModel.Languagezhcn = ApplicationLanguages.PrimaryLanguageOverride == "zh-Hans-CN";
        ViewModel.Languageenus = ApplicationLanguages.PrimaryLanguageOverride == "en-US";
        InitializeComponent();
    }

    private void RadioButton_Checked1(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.Languageenus = true;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        localSettings.SaveString("CurrentLanguage", "en-US");
        ApplicationLanguages.PrimaryLanguageOverride = "en-US";
    }
    
    private void RadioButton_Checked2(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.Languagezhcn = true;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        localSettings.SaveString("CurrentLanguage", "zh-Hans-CN");
        ApplicationLanguages.PrimaryLanguageOverride = "zh-Hans-CN";
    }
}
