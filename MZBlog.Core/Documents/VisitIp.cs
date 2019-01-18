using Dapper.Extensions;
using System;

namespace MZBlog.Core.Documents
{
    public class VisitIp
    {
        [ExplicitKey]
        public string Ip { get; set; }

        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public int VisitCount { get; set; }
        public DateTime FirstVisitTime { get; set; }
        public DateTime LastVisitTime { get; set; }
    }
}