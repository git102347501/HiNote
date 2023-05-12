using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiNote.Service.Models
{
    public class LoginOutput
    {
        public string access_token { get; set; }

        public string expires_in { get; set; }

        public string token_type { get; set; }

        public string scope { get; set; }
    }

    public class LoginErrOutput
    {
        public string error { get; set; }

        public string error_description { get; set; }
    }
}
