using System;
using System.Collections.Generic;
using System.Text;

namespace HiNote.Service.Models
{
    public class GetNoteListOutput
    {
        public Guid Id { get; set; }
        
        public Guid CategoryId { get; set; }

        public string Title { get; set; }
    }
}
