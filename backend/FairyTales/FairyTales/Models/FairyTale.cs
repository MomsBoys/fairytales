using System;
using FairyTales.Entities;

namespace FairyTales.Models
{
    public class FairyTale
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        
        private string _cover;
        public string Cover
        {
            get { return string.IsNullOrEmpty(_cover) ? string.Empty : $"{DbManager.RootPath}/{Name}/{_cover}"; }
            set { _cover = value; }
        }

        private string _audio;
        public string Audio
        {
            get { return string.IsNullOrEmpty(_audio) ? string.Empty : $"{DbManager.RootPath}/{Name}/{_audio}"; }
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

        private bool _isUserLiked;
        public bool IsUserLiked
        {
            get { return _isUserLiked; }
            set { _isUserLiked = value; }
        }

        private bool _isUserFavorited;
        public bool IsUserFavorited
        {
            get { return _isUserFavorited; }
            set { _isUserLiked = value; }
        }

        public DateTime Date { get; set; }
        public string Path { get; set; }

        public Author Author => DbManager.GetAuthorByTale(Id);
    }
}