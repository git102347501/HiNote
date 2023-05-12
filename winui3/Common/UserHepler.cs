using HiNote.Helpers;
using HiNote.Service.Contracts.Services;
using HiNote.Service.Models;
using HiNote.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;

namespace HiNote.Common
{
    public static class UserHepler
    {
        public static async Task<bool> RefUserInfoAsync()
        {
            var user = App.GetService<IUserService>();
            await user.GetCurrentUserInfoAsync();

            var userInfoViewModel = App.GetService<UserInfoViewModel>();
            if (CurrentUser.UserInfo != null)
            {
                userInfoViewModel.UserName = CurrentUser.UserInfo.userName;
                userInfoViewModel.Phone = CurrentUser.UserInfo.phone;

                var shellViewModel = App.GetService<ShellViewModel>();
                shellViewModel.LoginText = CurrentUser.UserInfo.userName;
                shellViewModel.LoginIcon = new SymbolIcon(Symbol.Contact2);
                shellViewModel.IsLogin = true;
                return true;
            }
            return false;
        }

        public static void LogOut()
        {
            var shellViewModel = App.GetService<ShellViewModel>();
            shellViewModel.LoginText = new ResourceLoader().GetString("LoginPageLoginBtn");
            shellViewModel.IsLogin = false;
            shellViewModel.LoginIcon = new SymbolIcon(Symbol.OtherUser);

            // 清空用户信息
            var userInfoViewModel = App.GetService<UserInfoViewModel>();
            userInfoViewModel.UserName = "";
            userInfoViewModel.Phone = "";

            // 清空内容编辑区
            var listDetailsViewModel = App.GetService<ListDetailsViewModel>();
            listDetailsViewModel.Clear();

            NoteHelper.Clear();

            ClearTokenSetting();
        }

        public static void RefTokenSetting()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.SaveString("AccessToken", CurrentUser.AccessToken);
            localSettings.SaveString("ExpirationTime", DateTime.Now.AddDays(90).ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public static void ClearTokenSetting()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.SaveString("AccessToken", "");
            localSettings.SaveString("ExpirationTime", "");
        }

        public static (object?, object?) GetTokenSetting()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var accessToken = localSettings.Values["AccessToken"];
            var expirationTime = localSettings.Values["ExpirationTime"]; ;
            return (accessToken, expirationTime);
        }
    }
}
