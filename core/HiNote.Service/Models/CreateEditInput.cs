using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiNote.Service.Models
{
    public class CreateEditInput
    {
        public int Mode { get; set; }

        public string Content { get; set; }
        
        public string Instruction { get; set; }
    }
}
