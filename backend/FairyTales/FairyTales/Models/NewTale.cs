using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FairyTales.Models
{
    public class NewTale
    {
        public int Tale_ID { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public Nullable<int> Author_ID { get; set; }
        public string Text { get; set; }
        public string Audio { get; set; }
        public string Cover { get; set; }
        public int Category_ID { get; set; }
        public int Type_ID { get; set; }
        public string Tag { get; set; }
        public int LikeCount { get; set; }
        public DateTime Date { get; set; }
        public string ShortDescription { get; set; } 
    }
}