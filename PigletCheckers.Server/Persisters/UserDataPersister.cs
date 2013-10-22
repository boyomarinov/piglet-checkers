namespace PigletCheckers.Server.Persisters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using PigletCheckers.Data;
    using PigletCheckers.Models;
    using PigletCheckers.Server.Models;

    public class UserDataPersister : BaseDataPersister
    {
        private const string SessionKeyChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const int SessionKeyLen = 50;

        private const string ValidUsernameChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_1234567890";
        private const string ValidNicknameChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_1234567890 -";
        private const int MinUsernameNicknameChars = 6;
        private const int MaxUsernameNicknameChars = 30;

        public static void CreateUser(string username, string nickname, string authCode)
        {
            ValidateUsername(username);
            ValidateNickname(nickname);
            ValidateAuthCode(authCode);
            using (GameDbContext context = new GameDbContext())
            {
                var usernameToLower = username.ToLower();
                var nicknameToLower = nickname.ToLower();

                var user = context.Users.FirstOrDefault(u => u.Username == usernameToLower || u.Nickname.ToLower() == nicknameToLower);

                if (user != null)
                {
                    if (user.Username.ToLower() == usernameToLower)
                    {
                        throw new ServerErrorException("Username already exists", "ERR_DUP_USR");
                    }
                    else
                    {
                        throw new ServerErrorException("Nickname already exists", "ERR_DUP_NICK");
                    }
                }

                user = new User()
                {
                    Username = usernameToLower,
                    Nickname = nickname,
                    AuthCode = authCode,
                    Score = 0
                };

                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public static string LoginUser(string username, string authCode, out string nickname)
        {
            ValidateUsername(username);
            ValidateAuthCode(authCode);
            var context = new GameDbContext();
            using (context)
            {
                var usernameToLower = username.ToLower();
                var user = context.Users.FirstOrDefault(u => u.Username == usernameToLower && u.AuthCode == authCode);
                if (user == null)
                {
                    throw new ServerErrorException("Invalid username or password", "ERR_INV_USR");
                }

                var sessionKey = GenerateSessionKey(user.Id);
                user.SessionKey = sessionKey;
                nickname = user.Nickname;
                context.SaveChanges();
                return sessionKey;
            }
        }

        public static int LoginUser(string sessionKey)
        {
            ValidateSessionKey(sessionKey);
            var context = new GameDbContext();
            using (context)
            {
                var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new ServerErrorException("Invalid user authentication", "INV_USR_AUTH");
                }

                return user.Id;
            }
        }

        public static void LogoutUser(string sessionKey)
        {
            ValidateSessionKey(sessionKey);
            var context = new GameDbContext();
            using (context)
            {
                var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new ServerErrorException("Invalid user authentication", "INV_USR_AUTH");
                }

                user.SessionKey = null;
                context.SaveChanges();
            }
        }

        public static IEnumerable<UserScoreModel> GetAllUserScores()
        {
            var context = new GameDbContext();
            using (context)
            {
                var userScores = context.Users.OrderByDescending(user => user.Score).Select(UserScoreModel.GetUserScoreModel).ToList();
                     
                return userScores;
            }
        }

        private static void ValidateSessionKey(string sessionKey)
        {
            if (sessionKey.Length != SessionKeyLen || sessionKey.Any(ch => !SessionKeyChars.Contains(ch)))
            {
                throw new ServerErrorException("Invalid Password", "ERR_INV_AUTH");
            }
        }

        private static string GenerateSessionKey(int userId)
        {
            StringBuilder keyChars = new StringBuilder(50);
            keyChars.Append(userId.ToString());
            while (keyChars.Length < SessionKeyLen)
            {
                int randomCharNum;
                lock (BaseDataPersister.Rand)
                {
                    randomCharNum = BaseDataPersister.Rand.Next(SessionKeyChars.Length);
                }

                char randomKeyChar = SessionKeyChars[randomCharNum];
                keyChars.Append(randomKeyChar);
            }

            string sessionKey = keyChars.ToString();
            return sessionKey;
        }

        private static void ValidateUsername(string username)
        {
            if (username == null || username.Length < MinUsernameNicknameChars || username.Length > MaxUsernameNicknameChars)
            {
                throw new ServerErrorException(string.Format("Username should be between {0} and {1} symbols long", MinUsernameNicknameChars, MaxUsernameNicknameChars), "INV_USR_LEN");
            }
            else if (username.Any(ch => !ValidUsernameChars.Contains(ch)))
            {
                throw new ServerErrorException("Username contains invalid characters", "INV_USR_CHARS");
            }
        }

        private static void ValidateNickname(string nickname)
        {
            if (nickname == null || nickname.Length < MinUsernameNicknameChars || nickname.Length > MaxUsernameNicknameChars)
            {
                throw new ServerErrorException(string.Format("Nickname should be between {0} and {1} symbols long", MinUsernameNicknameChars, MaxUsernameNicknameChars), "INV_NICK_LEN");
            }
            else if (nickname.Any(ch => !ValidNicknameChars.Contains(ch)))
            {
                throw new ServerErrorException("Nickname contains invalid characters", "INV_NICK_CHARS");
            }
        }

        private static void ValidateAuthCode(string authCode)
        {
            if (authCode.Length != GameDataPersister.Sha1CodeLength)
            {
                throw new ServerErrorException("Invalid user authentication", "INV_USR_AUTH");
            }
        }
    }
}