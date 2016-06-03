using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FairyTales.Entities;

namespace FairyTales
{
    public class FairyTale
    {
        public int Id { get; set; }
        public string Name { get; set; }

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

        public int LikeCount { get; set; }

        public FairyTale(Tale tale)
        {
            Id = tale.Tale_ID;
            Name = tale.Name;
            Text = tale.Text;
            Cover = tale.Cover;
            Audio = tale.Audio;
            LikeCount = tale.LikeCount;
            Date = tale.Date;
            Path = tale.Path;
        }

        public bool IsUserLiked { get; set; }

        public bool IsUserFavorited { get; set; }

        public DateTime Date { get; set; }
        public string Path { get; set; }

        public Author Author
        {
            get { return DbManager.GetAuthorByTale(Id); }
        }

        public List<Tag> Tags
        {
            get { return DbManager.GetTagsByTale(Id); }
        }
    }
}