using System.Collections.Generic;
using FairyTales.Entities;

namespace FairyTales.Models
{
    public class MainPageData
    {
        public  List<Tale> PopularTales;
        public  List<Tale> LatestTales;
 
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