using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Flour.Commons
{
    public static class StringExtensions
    {
        public static string Sha256(this string input)
        {
            using var hashManager = SHA256.Create();
            var hash = hashManager.ComputeHash(Encoding.UTF8.GetBytes(input));
            return GetStringFromHash(hash);
        }
        
        public static string Sha512(this string input)
        {
            using var hashManager = SHA512.Create();
            var hash = hashManager.ComputeHash(Encoding.UTF8.GetBytes(input));
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(IEnumerable<byte> hash)
        {
            var result = new StringBuilder();
            foreach (var t in hash)
            {
                result.Append(t.ToString("X2"));
            }

            return result.ToString();
        }
    }
}