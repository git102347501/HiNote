using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HiNote.Service.Models;

namespace HiNote.Core.Services
{
    public class BasicService
    {
        public HttpClient HttpClient;

        public const string AuthUrl = "https://www.auth.com";
        public const string ApiUrl = "https://www.api.com";

        public BasicService()
        {
            HttpClient = new HttpClient();
        }

        public bool IsAuth()
        {
            return !string.IsNullOrWhiteSpace(CurrentUser.AccessToken);
        }
    }
}
