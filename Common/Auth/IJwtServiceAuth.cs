namespace Common.Auth
{
    public interface IJwtServiceAuth
    {
        string GenerateToken(string userId, string role);
    }
}
