using System;
namespace MZBlog.Core.Documents
{
    [Serializable]
    public class Tag
    {
        public string Name { get; set; }

        public string Slug { get; set; }
    }
}