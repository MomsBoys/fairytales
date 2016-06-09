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

        public static List<FairyTale> GetSearchByAll(string text)
        {
            var taleByName = DbManager.GetSearchByTaleName(text);
            var taleByTag = DbManager.GetSearchByTag(text);
            var taleByAuthor = DbManager.GetSearchByAuthor(text);

            List<FairyTale> resultList = new List<FairyTale>();
            resultList.AddRange(taleByAuthor);
            resultList.AddRange(taleByTag);
            resultList.AddRange(taleByName);

            return resultList;
        }

        public static List<FairyTale> GetSearchByAuthor(string _authorLastName)
        {
            var context = new DBFairytaleEntities();
            var _searchAuthor = (from tale in context.Tales
                                 join author in context.Authors on tale.Author_ID equals author.Author_ID
                                 where author.LastName == _authorLastName
                                 select tale).ToList();
            var resultListAuthor = new List<FairyTale>();
            foreach (var item in _searchAuthor)
            {
                resultListAuthor.Add(new FairyTale(item));
            }
            return resultListAuthor;
        }

        public static List<FairyTale> GetSearchByTaleName(string _taleName)
        {
            var context = new DBFairytaleEntities();
            var _searchName = (from tale in context.Tales select tale).Where(t => t.Name == _taleName).ToList();
            var result = new List<FairyTale>();
            foreach (var tale in _searchName)
            {
                result.Add(new FairyTale(tale));
            }
            return result;
        }

        public static List<FairyTale> GetSearchByTag(string _tag)
        {
            var context = new DBFairytaleEntities();
            var _searchTag = (from tale_tag in context.Tale_Tag
                              join tag in context.Tags on tale_tag.Tag_ID equals tag.Tag_ID
                              join tale in context.Tales on tale_tag.Tale_ID equals tale.Tale_ID
                              where tag.Name == _tag
                              select tale).ToList();

            var resultListTag = new List<FairyTale>();
            foreach (var item in _searchTag)
            {
                resultListTag.Add(new FairyTale(item));
            }
            return resultListTag;
        }


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

        public static void PopulateUserLikesAndFavorites(ref List<FairyTale> tales, string userId)
        {
            var currentUser = CurrentUser(userId);

            if (currentUser == null)
                return;
            
            foreach (var tale in tales)
            {
                var userTale = currentUser.User_Tale.FirstOrDefault(innerUserTale => innerUserTale.Tale_ID == tale.Id);

                if (userTale != null)
                {
                    tale.IsUserLiked = userTale.IsLiked;
                    tale.IsUserFavorite = userTale.IsFavorite;
                }
            }
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

        public static void AddFairyTaleToFavorites(int fairyTaleId, string userId)
        {
            try
            {
                var dbContext = new DBFairytaleEntities();

                var userTale =
                    dbContext.User_Tale.FirstOrDefault(
                        innerUserTale => innerUserTale.Tale_ID == fairyTaleId && innerUserTale.User_ID.Equals(userId));

                if (userTale == null)
                    return;

                userTale.IsFavorite = !userTale.IsFavorite;
                dbContext.SaveChanges();
            }
            catch
            {
                Console.WriteLine(@"AddFairyTaleToFavorites-Exception");
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
