using HiNote.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiNote.Service.Contracts.Services
{
    public interface IUserService
    {
        Task<bool> GetPhoneCodeAsync(string phone);

        Task<ResultDto<LoginOutput>> LoginAsync(string phone, string code);

        Task<ResultDto<GetUserInfoOutput>> GetCurrentUserInfoAsync();

        Task<ResultDto<LoginOutput>> LoginPwdAsync(string account, string pwd);

        Task<ResultDto> Register(string phone, string userName, string pwd);
    }
}
