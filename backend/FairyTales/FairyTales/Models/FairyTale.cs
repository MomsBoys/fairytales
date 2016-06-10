using System;
using System.IO;
using System.Text;
using FairyTales.Entities;
using System.Collections.Generic;
using Type = FairyTales.Entities.Type;

namespace FairyTales.Models
{
    public class FairyTale
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public int LikesCount { get; set; }
        public bool IsUserLiked { get; set; }
        public bool IsUserFavorite { get; set; }
        public DateTime Date { get; set; }
        public string Path { get; set; }

        private string _text;
        public string Text
        {
            get
            {
                return
                    File.ReadAllText(
                        System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Data/" + Name + "/" + _text),
                        Encoding.Default);
            }
            set { _text = value; }
        }
        
        private string _cover;
        public string Cover
        {
            get { return string.IsNullOrEmpty(_cover) ? string.Empty : string.Format("{0}/{1}/{2}", DbManager.RootPath, Name, _cover); }
            set { _cover = value; }
        }

        private string _audio;
        public string Audio
        {
            get { return string.IsNullOrEmpty(_audio) ? string.Empty : string.Format("{0}/{1}/{2}", DbManager.RootPath, Name, _audio); }
            set { _audio = value; }
        }

        public Author Author { get; set; }
        public Type Type { get; set; }
        public Category Category { get; set; }
        
        public List<Tag> Tags
        {
            get { return DbManager.GetTagsByTale(Id); }
        }

        public FairyTale() { }

        public FairyTale(Tale tale)
        {
            Id = tale.Tale_ID;
            Name = tale.Name;
            Text = tale.Text;
            ShortDescription = tale.ShortDescription;
            Cover = tale.Cover;
            Audio = tale.Audio;
            LikesCount = tale.LikeCount;
            Date = tale.Date;
            Path = tale.Path;
            Author = tale.Author;
            Type = tale.Type;
            Category = tale.Category;
        }

        public string TextPath { get; set; }
        public string CoverPath { get; set; }
        public string AudioPath { get; set; }
        public string AuthorInput { get; set; }
        public string CategoryInput { get; set; }
        public string TypeInput { get; set; }
        public string[] SelectedTags { get; set; }
    }
}