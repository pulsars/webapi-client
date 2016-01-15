using System;
using System.Security.Cryptography;
using System.Text;
using ClientDirectory.Interfaces;

namespace ClientDirectory.Helpers
{
    public class SecurityHelper : ISecurityHelper
    {
        public bool ComparePasswords(string storedPassword, string storedSalt, string providedPass)
        {
            var providedPassword = HashPassword(providedPass, storedSalt);
            return storedPassword.Equals(providedPassword, StringComparison.Ordinal);
        }

        public string HashPassword(string password, string salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password + salt);
            var hashAlgoritm = MD5.Create();
            bytes = hashAlgoritm.ComputeHash(bytes);
            return Convert.ToBase64String(bytes);
        }

        public string SaltGenerator()
        {
            var rng = RandomNumberGenerator.Create();
            var buff = new byte[8];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }
    }
}
