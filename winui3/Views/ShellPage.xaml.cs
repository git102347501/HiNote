using HiNote.Common;
using HiNote.Contracts.Services;
using HiNote.Helpers;
using HiNote.Service.Models;
using HiNote.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.Resources;
using Windows.System;

namespace HiNote.Views;

// TODO: Update NavigationViewItem titles and icons in ShellPage.xaml.
public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel
    {
        get;
    }

    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        ViewModel.NavigationService.Frame = NavigationFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);

        // TODO: Set the title bar icon by updating /Assets/WindowIcon.ico.
        // A custom title bar is required for full window theme and Mica support.
        // https://docs.microsoft.com/windows/apps/develop/title-bar?tabs=winui3#full-customization
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        //App.MainWindow.Activated += MainWindow_Activated;
        //AppTitleBarText.Text = "AppDisplayName".GetLocalized();
        InitAsync();
    }

    private async void InitAsync()
    {
        var setting = UserHepler.GetTokenSetting();
        if (setting.Item2 != null && !string.IsNullOrWhiteSpace(setting.Item2.ToString()))
        {
            if (DateTime.TryParse(setting.Item2.ToString(), out DateTime expirationTimes))
            {
                if (expirationTimes <= DateTime.Now)
                {
                    UserHepler.ClearTokenSetting();
                }
            }
        }
        if (setting.Item1 != null)
        {
            CurrentUser.AccessToken = setting.Item1.ToString();
            var data = await UserHepler.RefUserInfoAsync();
            if (data)
            {
                var listDetailsViewModel = App.GetService<ListDetailsViewModel>();
                await listDetailsViewModel.LoadCategoryListAsync();
            }
        }
    }

    private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        TitleBarHelper.UpdateTitleBar(RequestedTheme);

        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        AppTitleBar.Margin = new Thickness()
        {
            Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom
        };
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue)
        {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        var navigationService = App.GetService<INavigationService>();

        var result = navigationService.GoBack();

        args.Handled = result;
    }

    /// <summary>
    /// 点击用户列表项目
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NavigationViewItem_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (!ViewModel.IsLogin)
        {
            this.Login();
        }
        else
        {
            this.ShowUserDetail();
        }
    }
    private string GetLocalString(string key)
    {
        return new ResourceLoader().GetString(key);
    }

    /// <summary>
    /// 展示用户详情
    /// </summary>
    private async void ShowUserDetail()
    {
        var dialog = new ContentDialog
        {
            Content = new UserInfoPage(),
            Title = GetLocalString("UserInfoDialogTitle"),
            IsSecondaryButtonEnabled = true,
            Width = 420,
            PrimaryButtonText = GetLocalString("Logout"),
            SecondaryButtonText = GetLocalString("Close"),
            XamlRoot = this.XamlRoot,
            RequestedTheme = this.ActualTheme
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            UserHepler.LogOut();
            Login();
        }
    }

    /// <summary>
    /// 登录
    /// </summary>
    private void Login()
    {
        var newWindow = WindowHelper.CreateWindow();
        var rootPage = new LoginPage();
        rootPage.RequestedTheme = ThemeHelper.RootTheme;
        newWindow.Content = rootPage;
        newWindow.SetWindowSize(430, 440);
        newWindow.SetIsAlwaysOnTop(true);
        newWindow.SetWindowPresenter(AppWindowPresenterKind.Overlapped);
        newWindow.ExtendsContentIntoTitleBar = true;
        newWindow.Activate();

        WindowId windowId = Win32Interop.GetWindowIdFromWindow(newWindow.GetWindowHandle());
        AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
        if (appWindow is not null)
        {
            DisplayArea displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest);
            if (displayArea is not null)
            {
                var CenteredPosition = appWindow.Position;
                CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
                CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
                appWindow.Move(CenteredPosition);
            }
        }
    }

}
