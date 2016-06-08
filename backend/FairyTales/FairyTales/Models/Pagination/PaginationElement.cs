using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FairyTales.Models.Pagination
{
    public class PaginationElement
    {
        public int Page { get; set; }
        public int CurrentPage { get; set; }
        public String CustomText { get; set; }
        public bool IsDisabled { get; set; }
    }
}