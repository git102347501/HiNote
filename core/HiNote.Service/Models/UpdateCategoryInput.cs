using System;
using System.Collections.Generic;
using System.Text;

namespace HiNote.Service.Models
{
    public class UpdateCategoryInput
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? ParentId { get; set; }
    }
    public class UpdateCategoryOrderNoInput
    {
        public List<UpdateCategoryOrderNoDto> Items { get; set; }
    }

    public class UpdateCategoryOrderNoDto
    {
        public Guid Id { get; set; }
        public int OrderNo { get; set; }
    }
    public class UpdateCategoryColorInput
    {
        public Guid Id { get; set; }

        public string Color { get; set; }
    }
    public class UpdateCategoryIconInput
    {
        public Guid Id { get; set; }

        public string Icon { get; set; }
    }
}
