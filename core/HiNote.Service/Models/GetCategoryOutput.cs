using System;
using System.Collections.Generic;
using System.Text;

namespace HiNote.Service.Models
{
    public class GetCategoryOutput
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Color { get; set; }

        public Guid? ParentId { get; set; }

        public int? OrderNo { get; set; }
    }
}
