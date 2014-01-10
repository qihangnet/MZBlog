using Nancy.Security;
using System.Collections.Generic;

namespace MZBlog.Web.Security
{
    public class BlogUserIdentity : IUserIdentity
    {
        private readonly string _username;
        private readonly string[] _claims;

        public BlogUserIdentity(string username, string[] claims = null)
        {
            _username = username;
            _claims = claims;
        }

        public string UserName
        {
            get { return _username; }
        }

        public IEnumerable<string> Claims
        {
            get { return _claims; }
        }
    }
}