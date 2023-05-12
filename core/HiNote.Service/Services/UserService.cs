using HiNote.Service.Contracts.Services;
using HiNote.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HiNote.Core.Services;

namespace HiNote.Service.Services
{
    public class UserService : BasicService, IUserService
    {
        public LoginOutput UserData { get; private set; }
        public UserService()
        {
        }

        /// <summary>
        /// 获取登录验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<bool> GetPhoneCodeAsync(string phone)
        {
            var uri = new Uri(
                string.Format(AuthUrl + "/api/app/sms/login-code?phone={0}",
                phone));
            try
            {
                HttpResponseMessage response = null;
                response = await HttpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Debug.WriteLine(@"\tSaveAsync ERROR {0}", response.RequestMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tSaveAsync ERROR {0}", ex.Message);
            }
            return false;
        }

        /// <summary>
        /// ids4 获取令牌
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<ResultDto<LoginOutput>> LoginAsync(string phone, string code)
        {
            var uri = new Uri(string.Format(AuthUrl + "/connect/token", string.Empty));
            try
            {
                var fromContent = new FormUrlEncodedContent(new[]
                {
                     new KeyValuePair<string,string>("grant_type","SMSGrantType"),
                     new KeyValuePair<string,string>("client_id","EasyNote_Mobile"),
                     new KeyValuePair<string,string>("phoneNumber",phone),
                     new KeyValuePair<string,string>("smsCode",code),
                     new KeyValuePair<string, string>("client_secret","easy@note")
                });
                HttpResponseMessage response = null;
                response = await HttpClient.PostAsync(uri, fromContent);

                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<LoginOutput>(res);
                    if (data != null && !string.IsNullOrWhiteSpace(data.access_token))
                    {
                        CurrentUser.AccessToken = data.access_token;
                    }
                    return new ResultDto<LoginOutput>(data);
                }
                else
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<LoginErrOutput>(res);
                    return new ResultDto<LoginOutput>(data.error_description);
                }
            }
            catch (Exception ex)
            {
                return new ResultDto<LoginOutput>(ex.Message);
            }
        }

        public async Task<ResultDto<GetUserInfoOutput>> GetCurrentUserInfoAsync()
        {
            if (!IsAuth())
            {
                return new ResultDto<GetUserInfoOutput>("未登录的用户!");
            }
            var uri = new Uri(string.Format(ApiUrl + "/api/app/user"));
            try
            {
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", CurrentUser.AccessToken);
                HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                var response = await HttpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<GetUserInfoOutput>(res);
                    if (data != null)
                    {
                        CurrentUser.UserInfo = data;
                    }
                    return new ResultDto<GetUserInfoOutput>(data);
                }
                else
                {
                    return new ResultDto<GetUserInfoOutput>("获取用户信息失败，请重新登录!");
                }
            }
            catch(Exception ex)
            {
                return new ResultDto<GetUserInfoOutput>("获取用户信息失败，请重新登录!");
            }
        }


        /// <summary>
        /// ids4 获取令牌
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<ResultDto<LoginOutput>> LoginPwdAsync(string account, string pwd)
        {
            var uri = new Uri(string.Format(AuthUrl + "/connect/token", string.Empty));
            try
            {
                var fromContent = new FormUrlEncodedContent(new[]
                {
                     new KeyValuePair<string,string>("grant_type","password"),
                     new KeyValuePair<string,string>("client_id","EasyNote_Mobile"),
                     new KeyValuePair<string,string>("username", account),
                     new KeyValuePair<string,string>("password", pwd),
                     new KeyValuePair<string, string>("client_secret","easy@note")
                });
                HttpResponseMessage response = null;
                response = await HttpClient.PostAsync(uri, fromContent);

                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<LoginOutput>(res);
                    if (data != null && !string.IsNullOrWhiteSpace(data.access_token))
                    {
                        CurrentUser.AccessToken = data.access_token;
                    }
                    return new ResultDto<LoginOutput>(data);
                }
                else
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<LoginErrOutput>(res);
                    return new ResultDto<LoginOutput>(data.error_description);
                }
            }
            catch (Exception ex)
            {
                return new ResultDto<LoginOutput>(ex.Message);
            }
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<ResultDto> Register(string phone, string userName, string pwd)
        {
            var uri = new Uri(string.Format(AuthUrl + "/api/app/user/register", string.Empty));
            try
            {
                var fromContent = new FormUrlEncodedContent(new[]
                {
                     new KeyValuePair<string,string>("phone", phone),
                     new KeyValuePair<string,string>("pwd", pwd),
                     new KeyValuePair<string,string>("userName", userName)
                });
                HttpResponseMessage response = null;
                response = await HttpClient.PostAsync(uri, fromContent);

                if (response.IsSuccessStatusCode)
                {
                    return new ResultDto("注册成功", true);
                }
                else
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<RegisterOutput>(res);
                    if (data != null && data.error != null && !string.IsNullOrWhiteSpace(data.error.message))
                    {
                        return new ResultDto(data.error.message, false);
                    }
                    return new ResultDto("注册失败", false);
                }
            }
            catch (Exception ex)
            {
                return new ResultDto(ex.Message, false);
            }
        }
    }
}
