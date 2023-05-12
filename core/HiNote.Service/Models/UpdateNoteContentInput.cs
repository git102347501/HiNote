using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiNote.Service.Models
{
    [Serializable]
    public class UpdateNoteContentInput
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
    }
}
