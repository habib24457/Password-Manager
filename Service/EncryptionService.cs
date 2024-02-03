using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace PasswordManager.Service
{
    public class EncryptionService
    {
        public string EncryptPassword(string plainTextPassword)
        {
            if (string.IsNullOrEmpty(plainTextPassword))
            {
                return null;
            }
            else
            {
                byte[] storePassword = ASCIIEncoding.ASCII.GetBytes(plainTextPassword);
                string encryptedPassword = Convert.ToBase64String(storePassword);
                return encryptedPassword;
            }
        }

        public string DecryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return null;
            }
            else
            {
                byte[] encryptedPassword = Convert.FromBase64String(password);
                string decryptedPassword = ASCIIEncoding.ASCII.GetString(encryptedPassword);
                return decryptedPassword;
            }
        }
    }
}
