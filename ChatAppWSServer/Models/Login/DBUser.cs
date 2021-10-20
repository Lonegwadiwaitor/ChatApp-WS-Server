using System;
using System.Collections.Generic;
using System.Linq;
using ChatAppWSServer.Models.User;
using ChatAppWSServer.PrivateServices;

namespace ChatAppWSServer.Models.Login
{
    public class DBUser : IUserCredentials
    {
        public ulong UID { get; set; }
        public string username { get; set; }
        public string hashed_password { get; set; }
        
        public ulong[] RelationshipUIDs { get; set; }

        public List<Token> tokens { get; set; } = new List<Token>();

        public DBUser GetHashedUserCredentials() =>
            this;

        public Token? GetValidLoginToken() =>
            GetOrGenerateLoginToken();

        public Token? GenerateValidLoginToken()
        {
            var dbSvc = Globals.GetService<DatabaseService>();

            return dbSvc.AddTokenToUID(UID);
        }

        public Token? GetOrGenerateLoginToken()
        {
            var validToken = tokens.Where(x => x.issueDate.AddMonths(6).Second > DateTime.Now.Second); // make sure the
                                                                                                    // token isn't more than a month old
            return validToken.FirstOrDefault() ?? GenerateValidLoginToken();
        }

        public bool InvalidateOldTokens()
        {
            var dbSvc = Globals.GetService<DatabaseService>();

            return tokens.Where(x => x.issueDate.AddMonths(6).Second < DateTime.Now.Second).All(dbSvc.DeleteToken);
        }
    }
}