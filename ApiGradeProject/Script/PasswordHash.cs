namespace ApiGradeProject.Script
{
    public class PasswordHash
    {
        private const string FixedSalt = "$2b$10$Y/Bk/Ct0KG9pGW49AMPKqu";

        public static string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password, FixedSalt);

        public static bool VerifyPassword(string inputPassword, string storedPassword)
        {
            inputPassword = BCrypt.Net.BCrypt.HashPassword(inputPassword, FixedSalt);
            return inputPassword == storedPassword;
        }
    }
}
