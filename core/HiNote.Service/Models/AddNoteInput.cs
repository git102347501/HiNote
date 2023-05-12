using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiNote.Service.Models
{
    public class AddNoteInput
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string Tags { get; set; }

        public Guid CategoryId { get; set; }

        public int EditType { get; set; }
    }

    public class AddNoteOutput
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public byte[] Content { get; set; }

        public string Tags { get; set; }

        public Guid CategoryId { get; set; }


        /// <summary>
        /// 编辑器类型
        /// </summary>
        public int EditType { get; set; }
    }
}
