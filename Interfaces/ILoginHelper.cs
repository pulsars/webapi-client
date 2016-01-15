using System;
using ClientDirectory.Models;

namespace ClientDirectory.Interfaces
{
    public interface ILoginHelper
    {
        Login CreateNewLogin(Guid clientId, string name);

        bool IsValidLogin(string storedPassword, string storedSalt, string providedPassword);
    }
}