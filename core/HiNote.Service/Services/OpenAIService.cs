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
        private readonly string apiKey = "youkey";

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
        
        public async Task<ChatOutput> ChatAsync(List<ChatInput> input)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // 设置请求参数
            var data = new
            {
                model = "gpt-3.5-turbo",
                messages = input
            };

            // 发起POST请求并获取响应
            var response = await client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", data);
            var jsonString = await response.Content.ReadAsStringAsync();

            // 解析响应JSON并输出生成的聊天响应
            var responseObject = JsonConvert.DeserializeObject<ChatOutput>(jsonString);
            return responseObject;
        }
    }
}
