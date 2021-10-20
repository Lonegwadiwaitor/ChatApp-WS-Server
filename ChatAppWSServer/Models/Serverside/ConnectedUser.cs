using ChatAppWSServer.Models.Login;

namespace ChatAppWSServer.Models.Serverside
{
    public class ConnectedUser
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public DBUser? user { get; set; } = null;
    }
}