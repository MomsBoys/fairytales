using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Remoting.Channels;
using System.Web.WebPages;
using FairyTales.Entities;
using Type = FairyTales.Entities.Type;

namespace FairyTales.Models
{
    public enum ResponseType
    {
        Success,
        Updated,
        Error,
        Exists,
        EmptyValues
    }

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

        public static List<Tag> GetTags()
        {
            return new DBFairytaleEntities().Tags.ToList();
        }

        public static List<Author> GetAuthors()
        {
            return new DBFairytaleEntities().Authors.ToList();
        }

        public static List<FairyTale> GetTales()
        {
            return new DBFairytaleEntities().Tales.ToList().Select(tale => new FairyTale(tale)).ToList();
        }

        public static List<AspNetUser> GetUsers()
        {
            return new DBFairytaleEntities().AspNetUsers.ToList();
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

        public static FairyTale GetTaleById(int id)
        {
            var fairyTale = new DBFairytaleEntities().Tales.FirstOrDefault(tale => tale.Tale_ID == id);
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

        #region Admin Panel
        public static ResponseType AddNewCategory(Category category)
        {
            try
            {
                var dbContext = new DBFairytaleEntities();

                var isCategoryExists = dbContext.Categories.Any(
                    innerCategory => innerCategory.Name.Equals(category.Name)
                );

                if (isCategoryExists)
                    return ResponseType.Exists;

                category.Category_ID = dbContext.Categories.Max(innerCategory => innerCategory.Category_ID) + 1;
                category.Tales = new List<Tale>();
                
                dbContext.Categories.Add(category);
                dbContext.SaveChanges();

                return ResponseType.Success;
            }
            catch
            {
                Console.WriteLine(@"AddNewCategoryWithName-Exception");
                return ResponseType.Error;
            }
        }

        public static ResponseType EditExistingCategory(Category category)
        {
            try
            {
                var dbContext = new DBFairytaleEntities();

                var currentCategory = dbContext.Categories.FirstOrDefault(
                    innerCategory => innerCategory.Category_ID == category.Category_ID
                );

                if (currentCategory == null)
                    return ResponseType.Error;

                currentCategory.Name = category.Name;
                dbContext.SaveChanges();
                return ResponseType.Updated;
            }
            catch
            {
                Console.WriteLine(@"EditExistingCategory-Exception");
                return ResponseType.Error;
            }
        }

        public static ResponseType AddNewAuthor(Author author)
        {
            try
            {
                var dbContext = new DBFairytaleEntities();

                var isAuthorExists = dbContext.Authors.Any(
                    innerAuthor => innerAuthor.FirstName.Equals(author.FirstName) && innerAuthor.LastName.Equals(author.LastName)
                );

                if (isAuthorExists)
                    return ResponseType.Exists;

                author.Author_ID = dbContext.Authors.Max(innerAuthor => innerAuthor.Author_ID) + 1;
                author.Tales = new List<Tale>();
                
                dbContext.Authors.Add(author);
                dbContext.SaveChanges();

                return ResponseType.Success;
            }
            catch
            {
                Console.WriteLine(@"AddNewAuthor-Exception");
                return ResponseType.Error;
            }
        }

        public static ResponseType EditExistingAuthor(Author author)
        {
            try
            {
                var dbContext = new DBFairytaleEntities();

                var currentAuthor = dbContext.Authors.FirstOrDefault(
                    innerCategory => innerCategory.Author_ID == author.Author_ID
                );

                if (currentAuthor == null)
                    return ResponseType.Error;

                currentAuthor.FirstName = author.FirstName;
                currentAuthor.LastName = author.LastName;
                dbContext.SaveChanges();

                return ResponseType.Updated;
            }
            catch
            {
                Console.WriteLine(@"EditExistingAuthor-Exception");
                return ResponseType.Error;
            }
        }

        public static ResponseType AddNewTag(Tag tag)
        {
            try
            {
                var dbContext = new DBFairytaleEntities();

                var isTagExists = dbContext.Tags.Any(
                    innerTag => innerTag.Name.Equals(tag.Name)
                );

                if (isTagExists)
                    return ResponseType.Exists;

                tag.Tag_ID = dbContext.Tags.Max(innerTag => innerTag.Tag_ID) + 1;

                dbContext.Tags.Add(tag);
                dbContext.SaveChanges();

                return ResponseType.Success;
            }
            catch
            {
                Console.WriteLine(@"AddNewTag-Exception");
                return ResponseType.Error;
            }
        }

        public static ResponseType EditExistingTag(Tag tag)
        {
            try
            {
                var dbContext = new DBFairytaleEntities();

                var currentTag = dbContext.Tags.FirstOrDefault(
                    innerTag => innerTag.Tag_ID == tag.Tag_ID
                );

                if (currentTag == null)
                    return ResponseType.Error;

                currentTag.Name = tag.Name;
                dbContext.SaveChanges();

                return ResponseType.Updated;
            }
            catch
            {
                Console.WriteLine(@"EditExistingTag-Exception");
                return ResponseType.Error;
            }
        }

        public static ResponseType EditExistingUser(AspNetUser user)
        {
            try
            {
                var dbContext = new DBFairytaleEntities();

                var currentUser = dbContext.AspNetUsers.FirstOrDefault(
                    innerUser => innerUser.Id == user.Id
                );

                if (currentUser == null)
                    return ResponseType.Error;

                currentUser.FirstName = user.FirstName;
                currentUser.SecondName = user.SecondName;
                currentUser.IsAdmin = user.IsAdmin;

                dbContext.SaveChanges();

                return ResponseType.Updated;
            }
            catch
            {
                Console.WriteLine(@"EditExistingUser-Exception");
                return ResponseType.Error;
            }
        }

        public static ResponseType AddNewTale(FairyTale tale)
        {
            try
            {
                var dbContext = new DBFairytaleEntities();

                var isTaleExists = dbContext.Tales.Any(
                    innerTale => innerTale.Path.Equals(tale.Path)
                );

                if (isTaleExists)
                    return ResponseType.Exists;

                var authorId = 1;
                foreach (var author in GetAuthors())
                {
                    var authorFullName = string.Format("{0} {1}", author.LastName, author.FirstName);

                    if (authorFullName.Contains(tale.AuthorInput))
                    {
                        authorId = author.Author_ID;
                        break;
                    }
                }

                var categoryId = GetCategories().First(category => category.Name.Equals(tale.CategoryInput)).Category_ID;
                var typeId = GetTypes().First(type => type.Name.Equals(tale.TypeInput)).Type_ID;

                var currentTale = new Tale
                {
                    Tale_ID = GetTales().Max(innerTale => innerTale.Id) + 1,
                    Cover = "img.jpg",
                    Text = "text.txt",
                    Author_ID = authorId,
                    Category_ID = categoryId,
                    Type_ID = typeId,
                    Date = DateTime.Now,
                    LikeCount = 0,
                    Path = tale.Path.Replace(" ", "_"),
                    ShortDescription = tale.ShortDescription,
                    Name = tale.Name,
                    Tale_Tag = new List<Tale_Tag>()
                };

                if (!tale.AudioPath.IsEmpty())
                    currentTale.Audio = "audio.mp3";

                if (tale.SelectedTags != null)
                {
                    var allTags = GetTags();

                    foreach (var selectedTagName in tale.SelectedTags)
                    {
                        foreach (var tag in allTags)
                        {
                            if (tag.Name.Equals(selectedTagName))
                            {
                                currentTale.Tale_Tag.Add(new Tale_Tag
                                {
                                    Tale_Tag_ID = dbContext.Tale_Tag.Max(taleTag => taleTag.Tale_Tag_ID) + 1,
                                    Tag_ID = tag.Tag_ID,
                                    Tale_ID = currentTale.Tale_ID
                                });
                            }
                        }
                    }
                }

                dbContext.Tales.Add(currentTale);
                dbContext.SaveChanges();

                return ResponseType.Success;
            }
            catch
            {
                Console.WriteLine(@"AddNewTale-Exception");
                return ResponseType.Error;
            }
        }
        #endregion // Admin Panel
    }
}
