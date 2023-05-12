using HiNote.Common;
using HiNote.Helpers;
using HiNote.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Resources;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HiNote.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {

        public LoginViewModel ViewModel
        {
            get;
        }

        public LoginPage()
        {
            ViewModel = App.GetService<LoginViewModel>();
            ViewModel.BtnText = GetLocalString("LoginPageLoginBtn");
            this.InitializeComponent();
        }

        private string GetLocalString(string key)
        {
            return new ResourceLoader().GetString(key);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var confirmText = GetLocalString("LoginPageRegisterDialogConfirm");
            if (string.IsNullOrWhiteSpace(ViewModel.Account))
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = GetLocalString("LoginPageRegisterDialogTitle"),
                    PrimaryButtonText = confirmText,
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
                return;
            }
            if (ViewModel.Account.Length != 11)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = GetLocalString("LoginPageRegisterDialogPhone"),
                    PrimaryButtonText = confirmText,
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
                return;
            }
            if (ViewModel.Code.Length < 6)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = GetLocalString("LoginPageRegisterDialogPwd"),
                    PrimaryButtonText = confirmText,
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
                return;
            }
            if (!ViewModel.IsAgree)
            {
                var dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = GetLocalString("LoginPageRegisterDialogAgree"),
                    PrimaryButtonText = confirmText,
                    CloseButtonText = GetLocalString("LoginPageRegisterDialogClose"),
                    DefaultButton = ContentDialogButton.Primary
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    ViewModel.IsAgree = true;
                    Login();
                }
            }
            else
            {
                Login();
            }
        }
        private async void Login()
        {
            var res = await this.ViewModel.Login();
            if (res.IsSuccess)
            {
                WindowHelper.GetWindowForElement(this)?.Close();
                var listDetailsViewModel = App.GetService<ListDetailsViewModel>();
                await listDetailsViewModel.LoadCategoryListAsync();
                UserHepler.RefTokenSetting();
                await UserHepler.RefUserInfoAsync();
            }
            else
            {
                var confirmText = GetLocalString("LoginPageRegisterDialogConfirm");
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = res.Message,
                    PrimaryButtonText = confirmText,
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
            }
        }

        /// <summary>
        /// 转到注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RegisterVisibility = Visibility.Visible;
            ViewModel.LoginVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 注册提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var confirmText = GetLocalString("LoginPageRegisterDialogConfirm");
            var phonePattern = @"^1[3-9]\d{9}$";
            if (!Regex.IsMatch(ViewModel.UserName, phonePattern))
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = GetLocalString("LoginPageRegisterDialogPhoneCheck1"),
                    PrimaryButtonText = confirmText,
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
                return;
            }
            if (ViewModel.Pwd != ViewModel.RetryPwd)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = GetLocalString("LoginPageRegisterDialogPwdCheck1"),
                    PrimaryButtonText = confirmText,
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
                ViewModel.RetryPwd = "";
                return;
            }
            if (string.IsNullOrWhiteSpace(ViewModel.Pwd))
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = GetLocalString("LoginPageRegisterDialogPwdCheck2"),
                    PrimaryButtonText = confirmText,
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
                return;
            }
            if (string.IsNullOrWhiteSpace(ViewModel.RetryPwd))
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = GetLocalString("LoginPageRegisterDialogPwdCheck3"),
                    PrimaryButtonText = confirmText,
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
                return;
            }
            var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$";
            if (!Regex.IsMatch(ViewModel.Pwd, passwordPattern))
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = GetLocalString("LoginPageRegisterDialogPwdCheck4"),
                    PrimaryButtonText = confirmText,
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
                return;
            }
            if (!this.ViewModel.IsRegisterAgree)
            {
                var dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = GetLocalString("LoginPageRegisterDialogAgreeCheck"),
                    PrimaryButtonText = confirmText,
                    CloseButtonText = GetLocalString("LoginPageRegisterDialogClose"),
                    DefaultButton = ContentDialogButton.Primary
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    ViewModel.IsRegisterAgree = true;
                    RegisterAsync();
                }
            }
            else
            {
                RegisterAsync();
            }
        }
        private async void RegisterAsync()
        {
            var res = await ViewModel.Register();
            if (res.IsSuccess)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = GetLocalString("LoginPageRegisterDialogSuccess"),
                    PrimaryButtonText = GetLocalString("LoginPageRegisterDialogConfirm"),
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
            }
            else
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = res.Message,
                    PrimaryButtonText = GetLocalString("LoginPageRegisterDialogConfirm"),
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
            }
        }

        private void BtnKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key != Windows.System.VirtualKey.Enter)
            {
                return;
            }
            if (ViewModel.RegisterVisibility == Visibility.Visible)
            {
                RegisterButton_Click(sender, e);
            }
            else
            {
                Button_Click(sender, e);
            }
        }
    }
}
