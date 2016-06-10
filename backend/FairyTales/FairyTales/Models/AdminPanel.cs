using System.Collections.Generic;
using FairyTales.Entities;

namespace FairyTales.Models
{
    public class AdminPanel
    {
        public List<FairyTale> Tales
        {
            get { return DbManager.GetTales(); }
        }

        public List<Category> Categories
        {
            get { return DbManager.GetCategories(); }
        }

        public List<Tag> Tags
        {
            get { return DbManager.GetTags(); }
        }

        public List<Author> Authors
        {
            get { return DbManager.GetAuthors(); }
        }

        public List<AspNetUser> Users
        {
            get { return DbManager.GetUsers(); }
        }

        public List<Type> Types
        {
            get { return DbManager.GetTypes(); }
        } 
    }
}