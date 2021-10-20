using System;
using ChatAppWSServer.PrivateServices;

namespace ChatAppWSServer.Models.Login
{
    public class Token
    {
        public DateTime issueDate { get; set; } = DateTime.Now;
        public string key { get; set; } = Globals.GetService<RandomService>().SecureRandomString(30);
    }
}