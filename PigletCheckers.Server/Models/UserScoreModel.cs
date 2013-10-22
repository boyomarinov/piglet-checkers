namespace PigletCheckers.Server.Models
{
    using System.Runtime.Serialization;
    using PigletCheckers.Models;

    [DataContract(Name = "UserScore")]
    public class UserScoreModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "nickname")]
        public string Nickname { get; set; }

        [DataMember(Name = "score")]
        public long Score { get; set; }

        public static UserScoreModel GetUserScoreModel(User user)
        {
            UserScoreModel userScoreModel = new UserScoreModel
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Score = user.Score
            };

            return userScoreModel;
        }
    }
}