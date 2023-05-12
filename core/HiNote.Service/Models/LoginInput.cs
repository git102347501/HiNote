using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiNote.Service.Models
{
    public class LoginInput
    {
        public string grant_type = "SMSGrantType";

        public string phoneNumber { get; set; }

        public string smsCode { get; set; }

        public string client_id = "HiNote_Mobile";

        public string client_secret = "hi@note";
    }
}
