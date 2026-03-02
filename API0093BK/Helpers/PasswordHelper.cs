using System.Security.Cryptography;
using System.Text;

namespace API0093BK.Helpers
{
    /// <summary>
    /// Вспомогательный класс для хеширования паролей
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Хеширование пароля с использованием SHA256
        /// </summary>
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        /// <summary>
        /// Проверка соответствия пароля хешу
        /// </summary>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hashedPassword;
        }
    }
}