using System;

namespace IPIP.Net
{
    public class IPFormatException : Exception
    {
        public IPFormatException(string name) : base(name)
        {
        }
    }
}