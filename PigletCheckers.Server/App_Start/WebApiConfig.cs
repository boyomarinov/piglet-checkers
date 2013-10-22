namespace PigletCheckers.Server
{
    using System.Web.Http;
    using System.Web.Http.Cors;
    
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "GameApi",
                routeTemplate: "api/game/{gameId}/{action}/{sessionKey}",
                defaults: new { controller = "game", gameId = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "CheckersApi",
                routeTemplate: "api/checkers/{gameId}/{action}/{sessionKey}",
                defaults: new { controller = "checkers" });

            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{action}/{sessionKey}",
               defaults: new { sessionKey = RouteParameter.Optional });

            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
        }
    }
}
