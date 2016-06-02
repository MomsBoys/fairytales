using System.Collections.Generic;
using System.Linq;
using FairyTales.Entities;
using Type = FairyTales.Entities.Type;
using System.Web.Mvc;

namespace FairyTales.Models
{
    public static class DbManager
    {
        public const string RootPath = @"http://localhost:1599/Content/Data";

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
                item.Cover = $"{RootPath}/{item.Name}/{item.Cover}";
                item.Text = $"{RootPath}/{item.Name}/{item.Text}";
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

        public static FairyTale GetTaleByPath(string path)
        {
            var fairyTale = new DBFairytaleEntities().Tales.FirstOrDefault(tale => tale.Path.Equals(path));
            return new FairyTale(fairyTale);
        }

        public static bool IsUserLikedTaleWithId(int id, string email)
        {
            var currentUser = CurrentUser(email);
            return currentUser != null && currentUser.User_Tale.First(userTale => userTale.Tale_ID == id).IsLiked;
        }

        public static bool IsUserFavoritedTaleWithId(int id, string email)
        {
            var currentUser = CurrentUser(email);
            return currentUser != null && currentUser.User_Tale.First(userTale => userTale.Tale_ID == id).IsFavorite;
        }

        public static AspNetUser CurrentUser(string email)
        {
            var users = new DBFairytaleEntities().AspNetUsers;
            return users.FirstOrDefault(user => user.UserName.Equals(email));
        }

        public static Author GetAuthorByTale(int id)
        {
            var tales = new DBFairytaleEntities().Tales;
            return tales.FirstOrDefault(tale => tale.Tale_ID == id)?.Author;
        }
    }
}