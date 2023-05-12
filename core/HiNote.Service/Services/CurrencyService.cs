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

public class CurrencyService: BasicService, ICurrencyService
{
    /// <summary>
    /// 获取账户详情
    /// </summary>
    /// <returns></returns>
    public async Task<ResultDto<GetCurrencyOutput>> GetAsync()
    {
        if (!IsAuth())
        {
            return new ResultDto<GetCurrencyOutput>("未登录的用户!");
        }
        var uri = new Uri(string.Format(ApiUrl + "/api/app/currency"));
        try
        {
            var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
            HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

            var response = await HttpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GetCurrencyOutput>(res);
                return new ResultDto<GetCurrencyOutput>(result);
            }
            else
            {
                return new ResultDto<GetCurrencyOutput>("添加失败，请重试!");
            }
        }
        catch
        {
            return new ResultDto<GetCurrencyOutput>("获取用户信息失败，请重新登录!");
        }
    }
}