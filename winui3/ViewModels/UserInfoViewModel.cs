using CommunityToolkit.Mvvm.ComponentModel;
using HiNote.Service.Contracts.Services;
using HiNote.Service.Models;
using Windows.ApplicationModel.Resources;

namespace HiNote.ViewModels
{
    public class UserInfoViewModel : ObservableRecipient
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

        private string _exchangeMsg;
        public string ExchangeMsg
        {
            get => _exchangeMsg;
            set => SetProperty(ref _exchangeMsg, value);
        }

        private decimal _aiCount = 0;
        public decimal AICount
        {
            get => _aiCount;
            set => SetProperty(ref _aiCount, value);
        }

        private decimal _usedCount = 0;
        public decimal UsedCount
        {
            get => _usedCount;
            set => SetProperty(ref _usedCount, value);
        }

        private double _aiUsedProcess = 0;
        public double AiUsedProcess
        {
            get => _aiUsedProcess;
            set => SetProperty(ref _aiUsedProcess, value);
        }

        private readonly IUserService _userService;
        private readonly ICurrencyService _currencyService;
        private readonly IExchangeCodeService _exchangeCodeService;

        public UserInfoViewModel(IUserService userService, 
            ICurrencyService currencyService, IExchangeCodeService exchangeCodeService)
        {
            _userService = userService;
            _currencyService = currencyService;
            _exchangeCodeService = exchangeCodeService;
        }

        public void LogOut()
        {
            this.UserName = "";
            this.Phone = "";
        }

        public async void GetCurrencyInfo()
        {
            var data = await this._currencyService.GetAsync();
            if (data.IsSuccess)
            {
                this.AICount = data.Data.Amount;
                this.UsedCount = data.Data.usedAmount;
                this.AiUsedProcess = data.Data.Amount > 0 ? (double)(data.Data.usedAmount / data.Data.Amount) : 0;
            }
        }

        private string GetLocalString(string key)
        {
            return new ResourceLoader().GetString(key);
        }


        /// <summary>
        /// 兑换时长
        /// </summary>
        /// <param name="key"></param>
        public async Task<ResultDto> ExchangeDurationAsync(string key)
        {
            ExchangeMsg = GetLocalString("UserInfoPageExchangeLoadingMsg");
            var data = await this._exchangeCodeService.ExchangeAsync(new ExchangeCodeInput() { Code = key });
            if (data.IsSuccess)
            {
                this.GetCurrencyInfo();
                ExchangeMsg = GetLocalString("UserInfoPageExchangeSuccessMsg");
            } 
            else
            {
                ExchangeMsg = GetLocalString("UserInfoPageExchangeValidMsg");
            }
            return data;
        }
    }
}
