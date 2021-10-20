using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ChatAppWSServer.Models;
using LiteDB;
using BCrypt.Net;
using ChatAppWSServer.Models.Login;

namespace ChatAppWSServer.PrivateServices
{
    public class DatabaseService
    {
        private readonly LiteDatabase _loginData;
        
        public DatabaseService()
        {
            _loginData = new LiteDatabase("loginData");
        }

        public ulong GetNextUsableUID()
        {
            var loginData = _loginData.GetCollection<DBUser>("loginData");

            var entries = loginData.FindAll();

            if (!entries.Any()) return 1;
            
            return entries.Select(x => x.UID).Max() + 1;
        }

        public bool LoginAttempt(UserCredentials userCredentials)
        {
            var loginData = _loginData.GetCollection<DBUser>("loginData");

            var userData = loginData.Find(x => x.username == userCredentials.username).FirstOrDefault();

            return userData != null && BCrypt.Net.BCrypt.Verify(userCredentials.password, userData.hashed_password);
        }

        public DBUser? GetUser(string username)
        {
            var loginData = _loginData.GetCollection<DBUser>("loginData");

            var results = loginData.Find(x => x.username == username).ToList();

            if (!results.Any())
                return null;

            return results.FirstOrDefault();
        }

        public Token? AddTokenToUID(ulong UID)
        {
            var loginData = _loginData.GetCollection<DBUser>("loginData");

            var results = loginData.Find(x => x.UID == UID).ToList();

            if (!results.Any()) return null;

            var result = results.FirstOrDefault();

            loginData.DeleteMany(x => x.UID == UID);

            var newToken = new Token();
            
            result?.tokens.Add(newToken);

            loginData.Insert(result);

            return newToken;
        }

        public bool RegisterAttempt(UserCredentials userCredentials)
        {
            var loginData = _loginData.GetCollection<DBUser>("loginData");

            if (loginData.Find(x => x.username == userCredentials.username).Any())
                return false;

            var creds = new DBUser
            {
                username = userCredentials.username,
                hashed_password = BCrypt.Net.BCrypt.HashPassword(userCredentials.password, 10),
                UID = GetNextUsableUID()
            };

            loginData.Insert(creds);

            return true;
        }

        public bool DeleteToken(Token token)
        {
            var loginData = _loginData.GetCollection<DBUser>("loginData");

            var user = loginData.Find(x => x.tokens.Contains(token)).FirstOrDefault();

            loginData.DeleteMany(x => x == user);

            user.tokens = user.tokens.Where(x => x != token).ToList();

            loginData.Insert(user);

            return true;
        }

        public bool AddRelationship(ulong UID, ulong potentialFriendUID)
        {
            var loginData = _loginData.GetCollection<DBUser>("loginData");

            var userDataEnumable = loginData.Find(x => x.UID == UID);

            var potentialFriendEnumable = loginData.Find(x => x.UID == potentialFriendUID);

            if (!potentialFriendEnumable.Any()) return false;

            if (!userDataEnumable.Any()) return false;

            var userData = userDataEnumable.FirstOrDefault();

            var potentialFriend = potentialFriendEnumable.FirstOrDefault();

            var relationships1 = userData.RelationshipUIDs.ToList();

            if (relationships1.Contains(potentialFriendUID)) return false;

            relationships1.Add(potentialFriendUID);

            userData.RelationshipUIDs = relationships1.ToArray();

            var relationships2 = potentialFriend.RelationshipUIDs.ToList();

            relationships2.Add(UID);

            potentialFriend.RelationshipUIDs = relationships2.ToArray();

            if (!loginData.Update(userData)) return false;
            
            return loginData.Update(potentialFriend);
        }
    }
}