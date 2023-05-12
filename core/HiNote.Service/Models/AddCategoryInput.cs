using System;
using System.Collections.Generic;
using System.Text;

namespace HiNote.Service.Models
{
    public class AddCategoryInput
    {
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
    }
    public class AddCategoryOutput
    {
        public Guid Id{ get; set; }
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
    }
}
