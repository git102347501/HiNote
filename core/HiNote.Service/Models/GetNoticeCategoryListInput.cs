using System;
using System.Collections.Generic;
using System.Text;

namespace HiNote.Service.Models
{
    public class GetNoticeCategoryListInput : PagedAndSortedResultRequestDto
    {
        public Guid? Id { get; set; }
    }
}
