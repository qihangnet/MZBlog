using FluentAssertions;
using Xunit;

namespace MZBlog.Core.Extensions.Tests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void String_with_chinese_or_sharp_to_slug()
        {
            "slug".ToSlug()
                .Should().Be("slug");

            "C#".ToSlug()
                .Should().Be("c-sharp");

            "中国".ToSlug()
                .Should().Be("zhong-guo");

            "Visual Studio".ToSlug()
                .Should().Be("visual-studio");

            "Visual Studio中国版".ToSlug()
                .Should().Be("visual-studio-zhong-guo-ban");

            "<html>".ToSlug()
                .Should().Be("html");

            "《书名》".ToSlug()
                .Should().Be("shu-ming");

            " 《书名》 是 ，、ViSual     STUdio 中国版！@￥%……&*（）——+【】[]$';,.".ToSlug()
                .Should().Be("shu-ming-shi-visual-studio-zhong-guo-ban-at-percent-and-dollar");
        }
    }
}