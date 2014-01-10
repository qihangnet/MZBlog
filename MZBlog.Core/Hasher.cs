using System.Security.Cryptography;
using System.Text;

namespace MZBlog.Core
{
    public class Hasher
    {
        public static string GetMd5Hash(string input)
        {
            if (input == null)
                input = string.Empty;
            byte[] data = Encoding.UTF8.GetBytes(input.Trim().ToLowerInvariant());
            using (var md5 = new MD5CryptoServiceProvider())
            {
                data = md5.ComputeHash(data);
            }

            var ret = new StringBuilder();
            foreach (byte b in data)
            {
                ret.Append(b.ToString("x2").ToLowerInvariant());
            }
            return ret.ToString();
        }
    }
}