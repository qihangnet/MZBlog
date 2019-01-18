using System.IO;

namespace IPIP.Net
{
    public class InvalidDatabaseException : IOException
    {
        public InvalidDatabaseException(string message) : base(message)
        {
        }
    }
}