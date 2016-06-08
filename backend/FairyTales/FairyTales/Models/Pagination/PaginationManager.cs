using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FairyTales.Entities;

namespace FairyTales.Models.Pagination
{
    public class PaginationManager
    {
        private int _currentPage = 0;
        private int _pageCount = 0;
        private int _talesPerPage = 5;
        private List<FairyTale> _tales;

        public int CurrentPage { get { return _currentPage; } set { _currentPage = value; } }
        public int PageCount { get { return _pageCount; } set { _pageCount = value; } }

        public int TalesPerPage
        {
            get
            {
                return _talesPerPage;
            }
            set
            {
                _talesPerPage = value;
                if (_tales != null)
                    PageCount = (int)Math.Ceiling((double)_tales.Count / _talesPerPage);
            }
        }

        public PaginationManager(List<FairyTale> tales)
        {
            _tales = tales;
            if (_tales != null)
            {
                PageCount = (int)Math.Ceiling((double)tales.Count/TalesPerPage);
            }
        }

        public List<FairyTale> GetPage(int pageNum)
        {
            if ( _tales == null)
            {
                CurrentPage = 0;
                return null;
            }
            if (pageNum > PageCount)
            {
                pageNum = PageCount;
            }
            if (pageNum < 1)
            {
                pageNum = 1;
            }
            CurrentPage = pageNum;
            return _tales.Skip((pageNum - 1)*TalesPerPage).Take(TalesPerPage).ToList();
        } 

    }
}