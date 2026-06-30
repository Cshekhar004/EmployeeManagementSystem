using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagement.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes =
                    Encoding.UTF8.GetBytes(password);

                byte[] hash =
                    sha256.ComputeHash(bytes);

                StringBuilder builder =
                    new StringBuilder();

                foreach (byte b in hash)
                {
                    builder.Append(
                        b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public static bool VerifyPassword(
            string enteredPassword,
            string storedHash)
        {
            string hashedEnteredPassword =
                HashPassword(enteredPassword);

            return hashedEnteredPassword == storedHash;
        }

        public static bool LooksHashed(string password)
        {
            return password.Length == 64 &&
                   password.All(c =>
                        "0123456789abcdef".Contains(
                            char.ToLower(c)));
        }
    }
}