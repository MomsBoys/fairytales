using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FairyTales.Entities;

namespace FairyTales
{
    public class MainPageData
    {
        public static List<Tale> PopularTales;
        public static List<Tale> LatestTales;

        static  MainPageData()
        {
            PopularTales = new List<Tale>();
            LatestTales = new List<Tale>(); 
        }

        public void LatestIn(List<Tale> latest)
        {
            LatestTales = latest;
        }
        public void PopularIn(List<Tale> popular)
        {
            PopularTales = popular;
        }
    } 
}