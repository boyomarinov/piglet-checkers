namespace PigletCheckers.Server.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;
    using PigletCheckers.Server.Persisters;

    public class UserMessagesController : BaseApiController
    {
        [HttpGet]
        [ActionName("all")]
        public HttpResponseMessage GetAllMessages(string sessionKey)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                var userId = UserDataPersister.LoginUser(sessionKey);
                var messages = UserMessagesDataPersister.GetAllMessages(userId);
                return messages;
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("unread")]
        public HttpResponseMessage GetUnreadMessages(string sessionKey)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                var userId = UserDataPersister.LoginUser(sessionKey);
                var messages = UserMessagesDataPersister.GetUnreadMessages(userId);
                return messages;
            });

            return responseMsg;
        }
    }
}