using System;

public class PasswordHa
{
	public PasswordHa()
	{
        private const string FixedSalt = "$2a$11$Xp128K9vR5f6Q7w8Oe";

        public static string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password, FixedSalt);

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            BCrypt.Net.BCrypt.HashPassword(inputPassword, FixedSalt);
            return inputPassword == storedPassword;
        }
    }
}
фыв