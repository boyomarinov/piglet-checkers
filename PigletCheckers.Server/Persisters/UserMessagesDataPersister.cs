namespace PigletCheckers.Server.Persisters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using PigletCheckers.Data;
    using PigletCheckers.Models;
    using PigletCheckers.Server.Models;

    public class UserMessagesDataPersister : PigletCheckers.Server.Persisters.BaseDataPersister
    {
        public static IEnumerable<MessageModel> GetAllMessages(int userId)
        {
            var context = new GameDbContext();
            using (context)
            {
                var user = GetUser(userId, context);
                var messages = user.UserMessages;

                var messageModels =
                                   (from msg in messages
                                    select new MessageModel()
                                    {
                                        State = msg.State,
                                        Text = msg.Text,
                                        GameId = msg.Game.Id,
                                        GameTitle = msg.Game.Title,
                                        Type = msg.Type
                                    }).ToList();

                var readMessageState = MessageState.Read;

                foreach (var msg in user.UserMessages)
                {
                    msg.State = readMessageState;
                }

                context.SaveChanges();

                return messageModels;
            }
        }

        public static IEnumerable<MessageModel> GetUnreadMessages(int userId)
        {
            var messages = GetAllMessages(userId).Where(msg => msg.State == MessageState.Unread);

            return messages;
        }
    }
}