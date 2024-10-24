using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace Server.Services
{
    public class PasswordService
    {
        public string Hash(string password)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] inputHash = SHA256.HashData(inputBytes);
            return Encoding.UTF8.GetString(inputHash);
        }

    }
}
