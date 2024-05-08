namespace UsersStudentsAPIApp.Security
{
    public class EncryptionUtil
    {
        public static string Encrypt(string clearText)
        {
            var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(clearText);
            return encryptedPassword;
        }

        public static bool IsValidPassword(string plainText, string cipherText)
        {
            var isValid = BCrypt.Net.BCrypt.Verify(plainText, cipherText);
            return isValid;
        }
    }
}
