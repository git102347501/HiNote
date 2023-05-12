using CommunityToolkit.Mvvm.ComponentModel;
using HiNote.Service.Contracts.Services;
using HiNote.Service.Models;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.Resources;

namespace HiNote.ViewModels
{
    public class LoginViewModel : ObservableRecipient
    {
        private readonly IUserService _userService;

        private Microsoft.UI.Xaml.Visibility loginVisibility = Visibility.Visible;
        public Microsoft.UI.Xaml.Visibility LoginVisibility
        {
            get => loginVisibility;
            set => SetProperty(ref loginVisibility, value);
        }

        private Microsoft.UI.Xaml.Visibility registerVisibility = Visibility.Collapsed;
        public Microsoft.UI.Xaml.Visibility RegisterVisibility
        {
            get => registerVisibility;
            set => SetProperty(ref registerVisibility, value);
        }

        private string _userName = "";
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private string _phone = "";
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        private string _pwd = "";
        public string Pwd
        {
            get => _pwd;
            set => SetProperty(ref _pwd, value);
        }

        private string _retryPwd = "";
        public string RetryPwd
        {
            get => _retryPwd;
            set => SetProperty(ref _retryPwd, value);
        }

        private string _account = "";
        public string Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }


        private string _code = "";
        public string Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }

        private bool isError = false;
        public bool IsError
        {
            get => isError;
            set => SetProperty(ref isError, value);
        }


        private bool isagree = false;
        public bool IsAgree
        {
            get => isagree;
            set => SetProperty(ref isagree, value);
        }

        private bool isregisteragree = false;
        public bool IsRegisterAgree
        {
            get => isregisteragree;
            set => SetProperty(ref isregisteragree, value);
        }

        private bool isstart = false;
        public bool IsStart
        {
            get => isstart;
            set => SetProperty(ref isstart, value);
        }

        private bool isloading = true;
        public bool IsLoading
        {
            get => isloading;
            set => SetProperty(ref isloading, value);
        }

        private string _btntext = "";
        public string BtnText
        {
            get => _btntext;
            set => SetProperty(ref _btntext, value);
        }

        private string _registerBtnText = "";
        public string RegisterBtnText
        {
            get => _registerBtnText;
            set => SetProperty(ref _registerBtnText, value);
        }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
            RegisterBtnText = GetLocalString("LoginPageRegisterBtn");
        }

        private string GetLocalString(string key)
        {
            return new ResourceLoader().GetString(key);
        }

        public async Task<ResultDto<LoginOutput>> Login()
        {
            IsError = false;
            IsStart = true;
            IsLoading = false;
            BtnText = GetLocalString("LoginPageLoginBtnLoading");

            var result = await _userService.LoginPwdAsync(Account, Code);
            if (result.IsSuccess)
            {
                await _userService.GetCurrentUserInfoAsync();
            }

            IsError = !result.IsSuccess;
            IsLoading = true;
            IsStart = false;
            BtnText = GetLocalString("LoginPageLoginBtn");

            return result;
        }

        public async Task<ResultDto> Register()
        {
            IsError = false;
            IsStart = true;
            IsLoading = false;
            RegisterBtnText = GetLocalString("LoginPageRegisterBtnLoading");

            var result = await _userService.Register(UserName, UserName, RetryPwd);
            if (result.IsSuccess)
            {
                this.Account = UserName;

                RegisterVisibility = Visibility.Collapsed;
                LoginVisibility = Visibility.Visible;
            }

            IsError = !result.IsSuccess;
            IsLoading = true;
            IsStart = false;
            RegisterBtnText = GetLocalString("LoginPageRegisterBtn");

            return result;
        }
    }
}