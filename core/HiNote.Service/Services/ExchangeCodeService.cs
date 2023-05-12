using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HiNote.Core.Services;
using HiNote.Service.Contracts.Services;
using HiNote.Service.Models;
using Newtonsoft.Json;

namespace HiNote.Service.Services;

public class ExchangeCodeService: BasicService, IExchangeCodeService
{
    /// <summary>
    /// 兑换
    /// </summary>
    /// <returns></returns>
    public async Task<ResultDto> ExchangeAsync(ExchangeCodeInput data)
    {
        if (!IsAuth())
        {
            return new ResultDto("未登录的用户!", false);
        }
        var uri = new Uri(string.Format(ApiUrl + "/api/app/exchange-code/exchange"));
        try
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
            HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

            var response = await HttpClient.PostAsync(uri, content);
            if (response.IsSuccessStatusCode)
            {
                return new ResultDto("兑换成功", true);
            }
            else
            {
                return new ResultDto("兑换失败，请重试!", false);
            }
        }
        catch
        {
            return new ResultDto("请求失败，请稍后再试!", false);
        }
    }
}