using System.Security.Cryptography;
using System.Text;

namespace project_managet_server
{
    public static class Extentions
    {
        public static string ComputeSHA256(string rawData)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
            return new(bytes.SelectMany(x => x.ToString("x2").ToCharArray()).ToArray());
        }
    }
}
