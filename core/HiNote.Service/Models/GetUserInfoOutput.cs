using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiNote.Service.Models
{
    public class GetUserInfoOutput
    {
        public FileInfo headerImg { get; set; }

        public string userName { get; set; }

        public string nickName { get; set; }

        public string phone { get; set; }

        public int sex { get; set; }

        public string province { get; set; }

        public string city { get; set; }

        public string area { get; set; }

        public string address { get; set; }

        public string detail { get; set; }

        public string weChat { get; set; }

        public DateTime? birthday { get; set; }

        public bool auth { get; set; }

        public bool vip { get; set; }

        public DateTime? vipExpirationDate { get; set; }

        public bool isNew { get; set; }

        public bool isIncomplete { get; set; }

        public int followCount { get; set; }

        public int fansCount { get; set; }

        public int newVisitorCount { get; set; }

        public int visitorCount { get; set; }

        public string id { get; set; }
    }
    public class FileInfo
    {
        public string fileId { get; set; }

        public string downloadMethod { get; set; }

        public string token { get; set; }

        public string expectedFileName { get; set; }

        public string downloadUrl { get; set; }

        public string superDownloadUrl { get; set; }
    }
}
