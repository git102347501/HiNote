using System;
using System.Collections.Generic;
using System.Text;

namespace HiNote.Service.Models
{
    public class GetNoteOutput
    {

        public string Title { get; set; }

        public byte[] Content { get; set; }
        
        public Guid CategoryId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        /// <summary>
        /// 编辑器类型
        /// </summary>
        public int EditType { get; set; }
    }
}
