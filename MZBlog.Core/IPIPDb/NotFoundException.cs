using System;

namespace IPIP.Net
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name) : base(name)
        {
        }
    }
}