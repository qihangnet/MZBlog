using Shouldly;
using Xunit;

namespace MZBlog.Core.Extensions.Tests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void String_with_chinese_or_sharp_to_slug()
        {
            "slug".ToSlug()
                .ShouldBe("slug");

            "C#".ToSlug()
                .ShouldBe("c-sharp");

            "中国".ToSlug()
                .ShouldBe("zhong-guo");

            "Visual Studio".ToSlug()
                .ShouldBe("visual-studio");

            "Visual Studio中国版".ToSlug()
                .ShouldBe("visual-studio-zhong-guo-ban");

            "<html>".ToSlug()
                .ShouldBe("html");

            "《书名》".ToSlug()
                .ShouldBe("shu-ming");

            " 《书名》 是 ，、ViSual     STUdio 中国版！@￥%……&*（）——+【】[]$';,.".ToSlug()
                .ShouldBe("shu-ming-shi-visual-studio-zhong-guo-ban-at-percent-and-dollar");
        }
    }
}