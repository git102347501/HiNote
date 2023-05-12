using System;
using System.Collections.Generic;
using System.Text;

namespace HiNote.Service.Models
{
    [Serializable]
    public class UpdateNoteInput
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; }

        public string Content { get; set; }

        public string Tags { get; set; }

        public Guid CategoryId { get; set; }


        /// <summary>
        /// 编辑器类型
        /// </summary>
        public int EditType { get; set; }
    }
}
