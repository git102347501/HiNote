using CommunityToolkit.Mvvm.ComponentModel;
using HiNote.Service.Contracts.Services;

namespace HiNote.ViewModels
{
    public class RegisterViewModel : ObservableRecipient
    {
        private string _userName;
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        private string _pwd;
        public string Pwd
        {
            get => _pwd;
            set => SetProperty(ref _pwd, value);
        }

        private string _retryPwd;
        public string RetryPwd
        {
            get => _retryPwd;
            set => SetProperty(ref _retryPwd, value);
        }

        private bool isagree;
        public bool IsAgree
        {
            get => isagree;
            set => SetProperty(ref isagree, value);
        }

        private bool isstart;
        public bool IsStart
        {
            get => isstart;
            set => SetProperty(ref isstart, value);
        }

        private string _btntext = "";
        public string BtnText
        {
            get => _btntext;
            set => SetProperty(ref _btntext, value);
        }


        private bool iserror;
        public bool IsError
        {
            get => iserror;
            set => SetProperty(ref iserror, value);
        }

        private bool isloading = true;
        public bool IsLoading
        {
            get => isloading;
            set => SetProperty(ref isloading, value);
        }

        private readonly IUserService _userService;

        public RegisterViewModel(IUserService userService)
        {
            _userService = userService;
        }

        public async void RegisterAsync()
        {
            //_userService.LoginAsync();
        }
    }
}
