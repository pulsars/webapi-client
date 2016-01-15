namespace ClientDirectory.Interfaces
{
    public interface ISecurityHelper
    {
        bool ComparePasswords(string storedPassword, string storedSalt, string providedPass);

        string HashPassword(string password, string salt);

        string SaltGenerator();
    }
}