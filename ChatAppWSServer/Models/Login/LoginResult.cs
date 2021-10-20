using MessagePack;

namespace ChatAppWSServer.Models.Login
{
    [MessagePackObject]
    public class LoginResult
    {
        [Key(0)]
        public Opcode result { get; set; } = Opcode.LOGIN_FAIL;
        [Key(1)]
        public string token { get; set; } = "";
    }
}