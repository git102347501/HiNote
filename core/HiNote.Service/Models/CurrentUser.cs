using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiNote.Service.Models
{
    public static class CurrentUser
    {
        public static string AccessToken { get; set; }

        public static GetUserInfoOutput UserInfo { get; set; }

        public static bool IsAuth()
        {
            return !string.IsNullOrWhiteSpace(AccessToken);
        }
    }
}
