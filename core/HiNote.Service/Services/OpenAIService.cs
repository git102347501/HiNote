using HiNote.Core.Services;
using HiNote.Service.Contracts.Services;
using HiNote.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HiNote.Service.Services
{
    public class OpenAIService : BasicService, IOpenAIService
    {
        /// <summary>
        /// 创建文本指令
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ResultDto<CreateEditOuput>> CreateEditAsync(CreateEditInput data)
        {
            if (!IsAuth())
            {
                return new ResultDto<CreateEditOuput>("未登录的用户!");
            }
            var uri = new Uri(string.Format(ApiUrl + "/api/app/open-aI/edit"));
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                var response = await HttpClient.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<CreateEditOuput>(res);
                    return new ResultDto<CreateEditOuput>(result);
                }
                else
                {
                    return new ResultDto<CreateEditOuput>("添加失败，请重试!");
                }
            }
            catch
            {
                return new ResultDto<CreateEditOuput>("获取用户信息失败，请重新登录!");
            }
        }
    }
}
