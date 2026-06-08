using System.Security.Cryptography;
using System.Text;

namespace Application.Shared.HelpFuntions
{
    public static class TokenHasher
    {
        public static string Hash(string rawToken)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            var res = Convert.ToHexString(bytes).ToLower();
            return res;
        }
    
        public static bool Verify(string rawToken, string hashedToken)
        {
            var hashedRawToken = Hash(rawToken);
            return hashedRawToken == hashedToken;
        }
    }
}
