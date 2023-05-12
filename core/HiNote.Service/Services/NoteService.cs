using HiNote.Core.Services;
using HiNote.Service.Contracts.Services;
using HiNote.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HiNote.Service.Services
{
    public class NoteService : BasicService, INoteService
    {
        public async Task<ResultDto<AddNoteOutput>> AddAsync(AddNoteInput data)
        {
            if (!IsAuth())
            {
                return new ResultDto<AddNoteOutput>("未登录的用户!");
            }
            var uri = new Uri(string.Format(ApiUrl + "/api/app/note"));
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
                    var result = JsonConvert.DeserializeObject<AddNoteOutput>(res);
                    return new ResultDto<AddNoteOutput>(result);
                }
                else
                {
                    return new ResultDto<AddNoteOutput>("添加失败，请重试!");
                }
            }
            catch
            {
                return new ResultDto<AddNoteOutput>("获取用户信息失败，请重新登录!");
            }
        }

        public async Task<ResultDto> UpdateAsync(UpdateNoteInput data)
        {
            if (!IsAuth())
            {
                return new ResultDto("未登录的用户!", false);
            }
            var uri = new Uri(string.Format(ApiUrl + "/api/app/note"));
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                var response = await HttpClient.PutAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    return new ResultDto("更新成功", true);
                }
                else
                {
                    return new ResultDto("更新失败，请重试!", false);
                }
            }
            catch(Exception ex)
            {
                return new ResultDto("获取用户信息失败，请重新登录!", false);
            }
        }

        public async Task<ResultDto> UpdateContentAsync(UpdateNoteContentInput data)
        {
            if (!IsAuth())
            {
                return new ResultDto("未登录的用户!", false);
            }
            var uri = new Uri(string.Format(ApiUrl + "/api/note/content"));
            try
            
            {
                using (HttpClient client = new HttpClient())
                {
                    var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                    client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                    HttpRequestMessage request;
                    using (request = new HttpRequestMessage(new HttpMethod("PUT"), uri))
                    {
                        var form = new MultipartFormDataContent();
                        form.Add(new StringContent(data.Id.ToString()), "id");
                        if (!string.IsNullOrWhiteSpace(data.Title))
                        {
                            form.Add(new StringContent(data.Title), "title");
                        }
                        form.Add(new StringContent(data.Content), "content");
                        request.Content = form;
                        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                        if (response.IsSuccessStatusCode)
                        {
                            return new ResultDto("更新成功", true);
                        }
                        else
                        {
                            var res = await response.Content.ReadAsStringAsync();
                            Debug.WriteLine(res);
                            return new ResultDto("更新失败，请重试!", false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new ResultDto("更新失败!", false);
            }
        }

        public async Task<ResultDto<GetNoteOutput>> GetAsync(Guid id)
        {
            var uri = new Uri(string.Format(ApiUrl + "/api/app/note/" + id, string.Empty));
            try
            {
                HttpResponseMessage response = null;
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                response = await HttpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<GetNoteOutput>(res);
                    return new ResultDto<GetNoteOutput>(data);
                }
                else
                {
                    return new ResultDto<GetNoteOutput>("获取信息失败");
                }
            }
            catch
            {
                return new ResultDto<GetNoteOutput>("获取用户信息失败，请重新登录!");
            }
        }

        public async Task<ResultDto<PagedResultDto<GetCategoryOutput>>> GetCategroyListAsync(GetNoticeCategoryListInput input)
        {
            var uri = new Uri(string.Format(ApiUrl + "/api/app/note/category-list?id={0}&Sorting={1}&SkipCount={2}&MaxResultCount={3}", input.Id,
                input.Sorting, input.SkipCount, input.MaxResultCount));
            try
            {
                HttpResponseMessage response = null;
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                response = await HttpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<PagedResultDto<GetCategoryOutput>>(res);
                    return new ResultDto<PagedResultDto<GetCategoryOutput>>(data);
                }
                else
                {
                    return new ResultDto<PagedResultDto<GetCategoryOutput>>("获取信息失败");
                }
            }
            catch
            {
                return new ResultDto<PagedResultDto<GetCategoryOutput>>("获取用户信息失败，请重新登录!");
            }
        }

        public async Task<ResultDto<PagedResultDto<GetNoteListOutput>>> GetNoteListAsync(GetNoteListInput input)
        {
            var urlstring = ApiUrl + $"/api/app/note?Id={input.Id}&SkipCount={input.SkipCount}&MaxResultCount={input.MaxResultCount}&Sorting={input.Sorting}";
            var uri = new Uri(string.Format(urlstring, string.Empty));
            try
            {
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                HttpResponseMessage response = await HttpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<PagedResultDto<GetNoteListOutput>>(res);
                    return new ResultDto<PagedResultDto<GetNoteListOutput>>(data);
                }
                else
                {
                    return new ResultDto<PagedResultDto<GetNoteListOutput>>("获取信息失败");
                }
            }
            catch
            {
                return new ResultDto<PagedResultDto<GetNoteListOutput>>("获取用户信息失败，请重新登录!");
            }
        }

        /// <summary>
        /// 添加日记目录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ResultDto<AddCategoryOutput>> AddCategoryAsync(AddCategoryInput data)
        {
            if (!IsAuth())
            {
                return new ResultDto<AddCategoryOutput>("未登录的用户!");
            }
            var uri = new Uri(string.Format(ApiUrl + "/api/app/note/category"));
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
                    var result = JsonConvert.DeserializeObject<AddCategoryOutput>(res);
                    return new ResultDto<AddCategoryOutput>(result);
                }
                else
                {
                    return new ResultDto<AddCategoryOutput>("更新失败，请重试!");
                }
            }
            catch
            {
                return new ResultDto<AddCategoryOutput>("获取用户信息失败，请重新登录!");
            }
        }

        /// <summary>
        /// 删除日记目录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultDto> DeleteCategoryAsync(Guid id)
        {
            if (!IsAuth())
            {
                return new ResultDto("未登录的用户!", false);
            }
            var uri = new Uri(string.Format(ApiUrl + $"/api/app/note/{id}/category"));
            try
            {
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                var response = await HttpClient.DeleteAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return new ResultDto("更新成功", true);
                }
                else
                {
                    return new ResultDto("更新失败，请重试!", false);
                }
            }
            catch
            {
                return new ResultDto("获取用户信息失败，请重新登录!", false);
            }
        }

        /// <summary>
        /// 删除日记目录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultDto> DeleteAsync(Guid id)
        {
            if (!IsAuth())
            {
                return new ResultDto("未登录的用户!", false);
            }
            var uri = new Uri(string.Format(ApiUrl + $"/api/app/note/{id}"));
            try
            {
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                var response = await HttpClient.DeleteAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return new ResultDto("更新成功", true);
                }
                else
                {
                    return new ResultDto("更新失败，请重试!", false);
                }
            }
            catch
            {
                return new ResultDto("获取用户信息失败，请重新登录!", false);
            }
        }

        /// <summary>
        /// 更新日记目录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ResultDto> UpdateCategoryAsync(UpdateCategoryInput data)
        {
            if (!IsAuth())
            {
                return new ResultDto("未登录的用户!", false);
            }
            var uri = new Uri(string.Format(ApiUrl + "/api/app/note/category"));
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                var response = await HttpClient.PutAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    return new ResultDto("更新成功", true);
                }
                else
                {
                    return new ResultDto("更新失败，请重试!", false);
                }
            }
            catch
            {
                return new ResultDto("获取用户信息失败，请重新登录!", false);
            }
        }

        /// <summary>
        /// 更新日记目录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ResultDto> UpdateCategoryOrderNoAsync(UpdateCategoryOrderNoInput data)
        {
            if (!IsAuth())
            {
                return new ResultDto("未登录的用户!", false);
            }
            var uri = new Uri(string.Format(ApiUrl + "/api/app/note/category-order-no"));
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                var response = await HttpClient.PutAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    return new ResultDto("更新成功", true);
                }
                else
                {
                    return new ResultDto("更新失败，请重试!", false);
                }
            }
            catch
            {
                return new ResultDto("获取用户信息失败，请重新登录!", false);
            }
        }

        /// <summary>
        /// 更新日记目录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ResultDto> UpdateCategoryColorAsync(UpdateCategoryColorInput data)
        {
            if (!IsAuth())
            {
                return new ResultDto("未登录的用户!", false);
            }
            var uri = new Uri(string.Format(ApiUrl + "/api/app/note/category-color"));
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                var response = await HttpClient.PutAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    return new ResultDto("更新成功", true);
                }
                else
                {
                    return new ResultDto("更新失败，请重试!", false);
                }
            }
            catch
            {
                return new ResultDto("获取用户信息失败，请重新登录!", false);
            }
        }

        /// <summary>
        /// 更新日记目录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ResultDto> UpdateCategoryIconAsync(UpdateCategoryIconInput data)
        {
            if (!IsAuth())
            {
                return new ResultDto("未登录的用户!", false);
            }
            var uri = new Uri(string.Format(ApiUrl + "/api/app/note/category-icon"));
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                var response = await HttpClient.PutAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    return new ResultDto("更新成功", true);
                }
                else
                {
                    return new ResultDto("更新失败，请重试!", false);
                }
            }
            catch
            {
                return new ResultDto("获取用户信息失败，请重新登录!", false);
            }
        }

        /// <summary>
        /// 更新日记目录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ResultDto> UpdateCategoryNameAsync(UpdateCategoryInput data)
        {
            if (!IsAuth())
            {
                return new ResultDto("未登录的用户!", false);
            }
            var uri = new Uri(string.Format(ApiUrl + "/api/app/note/category-name"));
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                var response = await HttpClient.PutAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    return new ResultDto("更新成功", true);
                }
                else
                {
                    return new ResultDto("更新失败，请重试!", false);
                }
            }
            catch
            {
                return new ResultDto("获取用户信息失败，请重新登录!", false);
            }
        }
    }
}
