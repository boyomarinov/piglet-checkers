namespace PigletCheckers.Server.Controllers
{
    using System.Net.Http;
    using System.Web.Http;
    using PigletCheckers.Server.Models;
    using PigletCheckers.Server.Persisters;

    public class UserController : BaseApiController
    {
        [HttpPost]
        [ActionName("register")]
        public HttpResponseMessage RegisterUser(UserRegisterModel user)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                UserDataPersister.CreateUser(user.Username, user.Nickname, user.AuthCode);
                string nickname = string.Empty;
                var sessionKey = UserDataPersister.LoginUser(user.Username, user.AuthCode, out nickname);
                return new UserLoggedModel()
                {
                    Nickname = nickname,
                    SessionKey = sessionKey
                };
            });

            return responseMsg;
        }

        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage LoginUser(UserLoginModel user)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                string nickname = string.Empty;
                var sessionKey = UserDataPersister.LoginUser(user.Username, user.AuthCode, out nickname);
                return new UserLoggedModel()
                {
                    Nickname = nickname,
                    SessionKey = sessionKey
                };
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("logout")]
        public HttpResponseMessage LogoutUser(string sessionKey)
        {
            var responseMsg = this.PerformOperation(() => UserDataPersister.LogoutUser(sessionKey));

            return responseMsg;
        }
    }
}