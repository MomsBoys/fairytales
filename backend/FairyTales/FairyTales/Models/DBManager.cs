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

        public static List<FairyTale> GetRecentReadedShortTales(string user)
        {
            var context = new DBFairytaleEntities();
            var queryTale = from fairytale in context.Tales
                join userTale in context.User_Tale on fairytale.Tale_ID equals userTale.Tale_ID
                where userTale.User_ID == user
                orderby userTale.Date descending 
                select fairytale;
            
            var tales = queryTale.ToList();
            return tales.Select(item => new FairyTale(item)).ToList();
        }

        public static List<FairyTale> GetFavouriteShortTales(string user)
        {
            var context = new DBFairytaleEntities();
            var queryTale = from fairytale in context.Tales
                            join userTale in context.User_Tale on fairytale.Tale_ID equals userTale.Tale_ID
                            where userTale.User_ID == user && userTale.IsFavorite
                            orderby userTale.Date descending
                            select fairytale;
            var tales = queryTale.ToList();
            return tales.Select(item => new FairyTale(item)).ToList();
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

        #region Tales Search

        public static List<FairyTale> GetSearchByAll(string text)
        {
            var taleByName = GetSearchByTaleName(text);
            var taleByTag = GetSearchByTag(text);
            var taleByAuthor = GetSearchByAuthor(text);

            List<FairyTale> resultList = new List<FairyTale>();
            resultList.AddRange(taleByAuthor);
            resultList.AddRange(taleByTag);
            resultList.AddRange(taleByName);

            return resultList.Distinct(new ProductComparer()).ToList();
        }

        public static List<FairyTale> GetSearchByAuthor(string authorName)
        {
            var context = new DBFairytaleEntities();
            var queryByAuthor = (from tale in context.Tales
                                 join author in context.Authors on tale.Author_ID equals author.Author_ID
                                 let nameAuth1 = author.FirstName + " " + author.LastName
                                 let nameAuth2 = author.LastName + " " + author.FirstName
                                 where nameAuth1.Contains(authorName) || nameAuth2.Contains(authorName)
                                 select tale).ToList();
            return queryByAuthor.Select(item => new FairyTale(item)).ToList();
        }

        public static List<FairyTale> GetSearchByTaleName(string taleName)
        {
            var context = new DBFairytaleEntities();
            var queryByName = (from tale in context.Tales
                               where tale.Name.Contains(taleName)
                               select tale).ToList();
            return queryByName.Select(tale => new FairyTale(tale)).ToList();
        }

        public static List<FairyTale> GetSearchByTag(string myTag)
        {
            var context = new DBFairytaleEntities();
            var queryByTag = (from tale_tag in context.Tale_Tag
                              join tag in context.Tags on tale_tag.Tag_ID equals tag.Tag_ID
                              join tale in context.Tales on tale_tag.Tale_ID equals tale.Tale_ID
                              where tag.Name.Contains(myTag)
                              select tale).ToList();
            return queryByTag.Select(item => new FairyTale(item)).ToList();
        }

        class ProductComparer : IEqualityComparer<FairyTale>
        {
            public bool Equals(FairyTale x, FairyTale y)
            {
                if (Object.ReferenceEquals(x, y)) return true;

                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                return x.Id == y.Id && x.Name == y.Name;
            }

   
            public int GetHashCode(FairyTale tale)
            {
                
                if (Object.ReferenceEquals(tale, null)) return 0;

                int hashTaleName = tale.Name == null ? 0 : tale.Name.GetHashCode();

                int hashTaleId = tale.Id.GetHashCode();

                return hashTaleName ^ hashTaleId;
            }

        }

        #endregion  // Tales Search

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

        public static List<FairyTale> GetRecommendedTale(string userId)
        {
            var context = new DBFairytaleEntities();
            var tales = context.Tales.Select(v => v).Where(t => t.User_Tale.Any(s => s.User_ID == userId && s.IsFavorite == true)).ToList();

            var tags = new List<TagCounter>();
            foreach (var item in tales)
            {
                foreach (var tag in item.Tale_Tag)
                {
                    if (tags.All(t => t.Tag_Id != tag.Tag_ID))
                    {
                        tags.Add(new TagCounter() {Count = 1, Tag_Id = tag.Tag_ID});
                    }
                    else
                    {
                        var el = tags.FirstOrDefault(t => t.Tag_Id == tag.Tag_ID);
                        if (el != null)
                            el.Count++;
                    }
                }
            }

            tags = tags.OrderByDescending(s => s.Count).Take(3).ToList();

            var recommendedTales = context.Tales.Select(w => w).ToList();
            recommendedTales = recommendedTales.Where(w => tales.All(a => a.Tale_ID != w.Tale_ID)).ToList();
            recommendedTales = recommendedTales.OrderByDescending(t => t.Tale_Tag.Count(v => tags.Any(e => e.Tag_Id == v.Tag_ID))).ToList();

            if (recommendedTales.Count != 0 &&
                tags.Count(c => recommendedTales[0].Tale_Tag.Any(a => a.Tag_ID == c.Tag_Id)) == 0)
            {
                // нема співпадінь по тегам - недостатньо казок в улюблених (або в базу додано), або теги погано написані (чи взагалі відсутні)
                //забиваємо все популярними казками
                recommendedTales =
                    recommendedTales.Where(r => tales.All(a => a.Tale_ID != r.Tale_ID))
                        .OrderByDescending(t => t.User_Tale.Count(u => u.IsFavorite))
                        .ToList();
            }
            return recommendedTales.Select(v => new FairyTale(v)).Take(5).ToList();
        }

        public class TagCounter
        {
            public int Tag_Id { get; set; }
            public int Count { get; set; }
        }
    }
}
