using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using FairyTales.Entities;
using Type = FairyTales.Entities.Type;

namespace FairyTales.Models
{
    public class DBManager
    { 
        private static String _rootPath = "http://localhost:1599/Content/Data";
         
        public static List<Tale> GetShortTales(List<Category> categories, List<Type> types)
        {
            DBFairytaleEntities context = new DBFairytaleEntities();
            List<Tale> a = context.Tales.Select(v => v).ToList();
            List<Tale> result = new List<Tale>();

            foreach (var item in a)
            {
                item.Cover = $"{_rootPath}/{item.Name}/{item.Cover}";
                item.Text = $"{_rootPath}/{item.Name}/{item.Text}";
                if ((categories == null || categories.Any(c => c.Category_ID == item.Category_ID))
                    && (types == null || types.Any(c => c.Type_ID == item.Type_ID)))
                {
                    result.Add(item);        
                }
            }
           // a = a.OrderBy(v => v.Name).Reverse().ToList();
            return result;
        }
        
        public static List<Category> GetCategories()
        {
            DBFairytaleEntities context = new DBFairytaleEntities();
            return context.Categories.Select(v => v).ToList(); 
        }

        public static List<Type> GetTypes()
        {
            DBFairytaleEntities context = new DBFairytaleEntities();
            return context.Types.Select(v => v).ToList(); 
        }
        
    }
}