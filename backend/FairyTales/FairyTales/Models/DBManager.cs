using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using FairyTales.Entities;
using Type = FairyTales.Entities.Type;

namespace FairyTales.Models
{
    public class DBManager
    {
        private static String _rootPath = "http://localhost:1599/Content/Data";
        
        public static MainPageData MainPagePopulateTales( )
        {
            DBFairytaleEntities context = new DBFairytaleEntities();
            
            List<Tale> latest = context.Tales.Select(v => v).OrderByDescending(tale => tale.Date).Take(5).ToList();

            foreach (var item in latest)
            {
                string readText = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Data/" + item.Name + "/" + item.Text), Encoding.Default);
                readText = readText.Remove(210, readText.Length - 210);
                readText += "...";
                item.Text = readText;
            }

            List<Tale> popular = context.Tales.Select(v => v).OrderByDescending(tale => tale.LikeCount).Take(4).ToList();
            
            MainPageData data = new MainPageData() ;
            data.PopularIn(popular);
            data.LatestIn(latest);

            return data;
        }

        public static List<Tale> GetShortTales(List<int> categories, List<int> types)
        {
            DBFairytaleEntities context = new DBFairytaleEntities();
            List<Tale> a = context.Tales.Select(v => v).ToList();
            List<Tale> result = new List<Tale>();

            foreach (var item in a)
            {
                if ((categories == null || categories.Count == 0 || categories.Any(c => c == item.Category_ID))
                    && (types == null || types.Count == 0 || types.Any(c => c == item.Type_ID)))
                {
                    result.Add(item);
                }
            }

            foreach (var item in result)
            {
                item.Cover = $"{_rootPath}/{item.Name}/{item.Cover}";
                string readText = File.ReadAllText($"{System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Data")}/{item.Name}/{item.Text}", Encoding.Default);
                readText = readText.Remove(210, readText.Length - 210);
                readText += "...";
                item.Text = readText;
            }

            return result;
        }

        public static List<Category> GetCategories()
        {
            DBFairytaleEntities context = new DBFairytaleEntities();
            return context.Categories.Select(v => v).ToList();
        }

        public static List<Tale> GetNewShortTales(List<int> categories, List<int> types)
        {
            return GetShortTales(categories, types).OrderByDescending(c=>c.Date).ToList();
        }

        public static List<Tale> GetPopularShortTales(List<int> categories, List<int> types)
        {
            return GetShortTales(categories, types).OrderByDescending(c=>c.LikeCount).ToList();
        }

        public static List<Type> GetTypes()
        {
            DBFairytaleEntities context = new DBFairytaleEntities();
            return context.Types.Select(v => v).ToList();
        }
    }
}
