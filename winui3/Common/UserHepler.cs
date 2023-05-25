using HiNote.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;

namespace HiNote.Common
{
    public static class UserHepler
    {
        public static void LogOut()
        {
            var shellViewModel = App.GetService<ShellViewModel>();
            shellViewModel.LoginText = new ResourceLoader().GetString("LoginPageLoginBtn");
            shellViewModel.IsLogin = false;
            shellViewModel.LoginIcon = new FontIcon();

            // 清空内容编辑区
            var listDetailsViewModel = App.GetService<ListDetailsViewModel>();
            listDetailsViewModel.Clear();

            NoteHelper.Clear();
        }
    }
}
