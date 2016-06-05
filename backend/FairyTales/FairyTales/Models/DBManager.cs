using System;
using System.Collections.Generic;
using System.Linq;
using FairyTales.Entities;
using Type = FairyTales.Entities.Type;

namespace FairyTales.Models
{
    public static class DbManager
    {
        public const string RootPath = @"http://localhost:1599/Content/Data";

        public static AspNetUser CurrentUser(string userId)
        {
            var users = new DBFairytaleEntities().AspNetUsers;
            return users.FirstOrDefault(user => user.Id.Equals(userId));
        }

        #region Tales Library Functionality
        public static MainPageData MainPagePopulateTales()
        {
            var latest = GetRecentShortTales(5);
            var popular = GetPopularShortTales(4);
            
            return new MainPageData
            {
                LatestTales = latest,
                PopularTales = popular
            };
        }
        public static List<FairyTale> GetShortTales(List<int> categories, List<int> types)
        {
            var context = new DBFairytaleEntities();
            var tales = context.Tales.Select(v => v).ToList();
            var result = new List<FairyTale>();

            foreach (var item in tales)
            {
                if ((categories == null || categories.Count == 0 || categories.Any(c => c == item.Category_ID))
                    && (types == null || types.Count == 0 || types.Any(c => c == item.Type_ID)))
                {
                    result.Add(new FairyTale(item));
                }
            }

            return result;
        }

        public static List<Category> GetCategories()
        {
            var context = new DBFairytaleEntities();
            return context.Categories.Select(v => v).ToList();
        }

        public static List<FairyTale> GetPopularShortTales(int talesCount)
        {
            return GetShortTales(null, null).OrderByDescending(c => c.LikesCount).Take(talesCount).ToList();
        }

        public static List<FairyTale> GetRecentShortTales(int talesCount)
        {
            return GetShortTales(null, null).OrderByDescending(c => c.Date).Take(talesCount).ToList();
        }

        public static List<FairyTale> GetNewShortTales(List<int> categories, List<int> types)
        {
            return GetShortTales(categories, types).OrderByDescending(c => c.Date).ToList();
        }

        public static List<FairyTale> GetPopularShortTales(List<int> categories, List<int> types)
        {
            return GetShortTales(categories, types).OrderByDescending(c => c.LikesCount).ToList();
        }

        public static List<Type> GetTypes()
        {
            return new DBFairytaleEntities().Types.Select(v => v).ToList();
        }
        #endregion // Tales Library Functionality

        #region Tale Functionality
        public static bool ValidateTaleByPath(string path)
        {
            return new DBFairytaleEntities().Tales.Any(tale => tale.Path.Equals(path));
        }

        public static FairyTale GetTaleByPath(string path)
        {
            var fairyTale = new DBFairytaleEntities().Tales.FirstOrDefault(tale => tale.Path.Equals(path));
            return new FairyTale(fairyTale);
        }

        public static Author GetAuthorByTale(int taleId)
        {
            var tale = new DBFairytaleEntities().Tales.FirstOrDefault(innerTale => innerTale.Tale_ID == taleId);

            if (tale == null)
                return null;

            return tale.Author;
        }

        public static List<Tag> GetTagsByTale(int id)
        {
            var dbModel = new DBFairytaleEntities();
            var tale = dbModel.Tales.First(inTale => inTale.Tale_ID == id);
            return tale.Tale_Tag.ToList().Select(taleTag => dbModel.Tags.First(tag => tag.Tag_ID == taleTag.Tag_ID)).ToList();
        }

        public static void AddFairyTaleToReadList(int fairyTaleId, string userId)
        {
            try
            {
                var dbContext = new DBFairytaleEntities();

                var isUserTaleExists =
                    dbContext.User_Tale.Any(
                        innerUserTale => innerUserTale.Tale_ID == fairyTaleId && innerUserTale.User_ID.Equals(userId));

                if (isUserTaleExists)
                    return;

                var userTale = new User_Tale
                {
                    Tale_ID = fairyTaleId,
                    User_ID = userId,
                    Date = DateTime.Now,
                    IsReaded = true,
                    IsLiked = false,
                    IsFavorite = false
                };

                dbContext.User_Tale.Add(userTale);
                dbContext.SaveChanges();
            }
            catch
            {
                Console.WriteLine(@"AddFairyTaleToReadList-Exception");
            }
        }

        public static User_Tale GetUserTaleByUserId(int taleId, string userId)
        {
            var currentUser = CurrentUser(userId);

            if (currentUser == null)
                return null;

            return currentUser.User_Tale.FirstOrDefault(innerUserTale => innerUserTale.Tale_ID == taleId);
        }

        public static bool IsUserLikedTaleWithId(int taleId, string userId)
        {
            var userTale = GetUserTaleByUserId(taleId, userId);
            return userTale != null && userTale.IsLiked;
        }

        public static bool IsUserFavoriteTaleWithId(int taleId, string userId)
        {
            var userTale = GetUserTaleByUserId(taleId, userId);
            return userTale != null && userTale.IsFavorite;
        }
        #endregion // Tale Functionality
    }
}
