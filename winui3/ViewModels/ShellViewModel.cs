using CommunityToolkit.Mvvm.ComponentModel;

using HiNote.Contracts.Services;
using HiNote.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.Resources;

namespace HiNote.ViewModels;

public class ShellViewModel : ObservableRecipient
{
    private bool _isBackEnabled;
    private object? _selected;

    private string _loginText = "";
    public string LoginText
    {
        get => _loginText;
        set => SetProperty(ref _loginText, value);
    }

    private IconElement _loginIcon = new FontIcon();
    public IconElement LoginIcon
    {
        get => _loginIcon;
        set => SetProperty(ref _loginIcon, value);
    }

    private bool _isLogin;
    public bool IsLogin
    {
        get => _isLogin;
        set => SetProperty(ref _isLogin, value);
    }

    public INavigationService NavigationService
    {
        get;
    }

    public INavigationViewService NavigationViewService
    {
        get;
    }

    public bool IsBackEnabled
    {
        get => _isBackEnabled;
        set => SetProperty(ref _isBackEnabled, value);
    }

    public object? Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

    public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
        LoginText = new ResourceLoader().GetString("LoginPageLoginBtn");
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;

        if (e.SourcePageType == typeof(SettingsPage))
        {
            Selected = NavigationViewService.SettingsItem;
            return;
        }

        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null)
        {
            Selected = selectedItem;
        }
    }
}
