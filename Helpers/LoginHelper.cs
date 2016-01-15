using ClientDirectory.Interfaces;
using ClientDirectory.Models;
using System;
using System.Linq;

namespace ClientDirectory.Helpers
{
    public class LoginHelper : ILoginHelper
    {
        private readonly ClientDirectoryContext _context;
        private readonly ISecurityHelper _securityHelper;

        public LoginHelper(ClientDirectoryContext context, ISecurityHelper securityHelper)
        {
            _context = context;
            _securityHelper = securityHelper;
        }

        public Login CreateNewLogin(Guid clientId, string name)
        {
            // Creates the login details for the new user.
            var salt = _securityHelper.SaltGenerator();

            // Default password
            var password = _securityHelper.HashPassword("EasyPassword123", salt);
            var username = CreateNewUsername(name);

            var newLogin = new Login
            {
                IdClient = clientId,
                Password = password,
                Salt = salt,
                Username = username
            };

            return newLogin;
        }

        public bool IsValidLogin(string storedPassword, string storedSalt, string providedPassword)
        {
            var passwordMatch = _securityHelper.ComparePasswords(storedPassword, storedSalt, providedPassword);
            return passwordMatch;
        }

        private string CreateNewUsername(string name)
        {
            // Based on the client name the username is created.
            // First tries to concatenate the first name and the last name (if provided)
            var username = name.Replace(" ", string.Empty);

            // Generate a new username until is unique.
            while (true)
            {
                var usernameExist = _context.Login.FirstOrDefault(a => a.Username == username);

                if (usernameExist == null)
                {
                    break;
                }

                // TODO: Improve username generator.
                username = string.Concat(username, DateTime.Now.Ticks);
            }

            return username;
        }
    }
}