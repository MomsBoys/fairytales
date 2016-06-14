using System.Web.Mvc;
using System.Web.Routing;

namespace FairyTales
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "FairyTale",
                url: "tales/{path}",
                defaults: new { controller = "FairyTale", action = "Index", path = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "TaleFavoriteAction",
                url: "tales/{path}/favorite",
                defaults: new { controller = "FairyTale", action = "FavoriteAction", path = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LikeAction",
                url: "tales/{path}/like",
                defaults: new { controller = "FairyTale", action = "LikeAction", path = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
