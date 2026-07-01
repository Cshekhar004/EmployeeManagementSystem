using System.Security.Cryptography;
using EmployeeManagement.Helpers;

namespace EmployeeManagement.Helpers
{
    public static class RememberMeHelper
    {
        public static string GenerateToken()
        {
            byte[] tokenBytes =
                RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(tokenBytes);
        }

        public static string HashToken(string token)
        {
            return PasswordHelper.HashPassword(token);
        }
    }
}