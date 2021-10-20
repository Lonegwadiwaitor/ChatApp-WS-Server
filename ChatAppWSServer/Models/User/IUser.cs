using ChatAppWSServer.Models.Login;

namespace ChatAppWSServer.Models.User
{
    public interface IUserCredentials
    {
        DBUser GetHashedUserCredentials();
        Token? GetValidLoginToken();
        Token? GenerateValidLoginToken();
        Token? GetOrGenerateLoginToken();
        bool InvalidateOldTokens();
    }
}