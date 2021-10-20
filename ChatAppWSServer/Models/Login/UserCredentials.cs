using MessagePack;

namespace ChatAppWSServer.Models.Login
{
    [MessagePackObject]
    public struct UserCredentials
    {
        [Key(0)]
        public ulong UID { get; set; }
        [Key(1)]
        public string username { get; set; }
        [Key(2)]
        public string password { get; set; }
    }
}