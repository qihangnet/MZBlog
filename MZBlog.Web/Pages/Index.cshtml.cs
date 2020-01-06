using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MZBlog.Core.Queries.Home;
using System.Threading.Tasks;

namespace MZBlog.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IPIP.Net.City _ipCity;

        public IndexModel(IMediator mediator, IPIP.Net.City ipCity)
        {
            _mediator = mediator;
            _ipCity = ipCity;
        }

        public RecentBlogPostsViewModel RecentBlogPosts { get; set; }

        public async Task OnGetAsync()
        {
            var pageNo = 1;
            if (Request.Query.ContainsKey("page"))
            {
                int.TryParse(Request.Query["page"], out pageNo);
            }
            var query = new RecentBlogPostsQuery() { Page = pageNo, Take = 10 };
            RecentBlogPosts = await _mediator.Send(query);
            // var ipAddress = Request.HttpContext.Connection.RemoteIpAddress;
            // if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            // {
            //     var ss = _ipCity.Find(ipAddress.ToString());//"115.159.114.205");
            //     if (ss.Length > 0)
            //         System.Console.WriteLine(string.Join(",", ss));
            //     else
            //         System.Console.WriteLine("无数据");
            // }
        }
    }
}