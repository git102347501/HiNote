using HiNote.Common;
using HiNote.Helpers;
using HiNote.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;
using System.Windows;
using Windows.UI.Popups;
using Windows.UI.Core;
using System.Text.RegularExpressions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HiNote.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserInfoPage : Page
    {
        public UserInfoViewModel ViewModel
        {
            get;
        }

        public UserInfoPage()
        {
            ViewModel = App.GetService<UserInfoViewModel>();
            ViewModel.ExchangeMsg = "";
            ViewModel.GetCurrencyInfo();
            this.InitializeComponent();
        }

        private string GetLocalString(string key)
        {
            return new ResourceLoader().GetString(key);
        }

        /// <summary>
        /// 兑换码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (keyTextBox.Text.Length < 12)
            {
                ViewModel.ExchangeMsg = GetLocalString("UserInfoPageExchangeErrorMsg");
                return;
            }
            var res = await ViewModel.ExchangeDurationAsync(keyTextBox.Text);
            if (res.IsSuccess)
            {
                keyTextBox.Text = "";
            }
        }

        private bool ValidateForm()
        {
            // 获取表单字段的值
            var value = keyTextBox.Text;

            // 验证字段值是否小于8位数或大于24位数
            if (value.Length < 8 || value.Length > 24)
            {
                return false;
            }

            return true;
        }
    }
}
