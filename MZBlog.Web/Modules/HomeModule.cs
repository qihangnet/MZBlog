using MZBlog.Core;
using MZBlog.Core.Commands.Posts;
using MZBlog.Core.ViewProjections.Home;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;
using System;
using System.Linq;

namespace MZBlog.Web.Modules
{
    public class HomeModule : FrontModule
    {
        private readonly ICommandInvokerFactory _commandInvokerFactory;

        public HomeModule(IViewProjectionFactory viewFactory, ISpamShieldService spamShield, ICommandInvokerFactory commandInvokerFactory)
            : base(viewFactory)
        {
            _viewFactory = viewFactory;
            _commandInvokerFactory = commandInvokerFactory;

            Get["/"] = p =>
                ReturnHomeAction(new RecentBlogPostsBindingModel() { Page = 1, Take = 10 });

            Get["/page/{page:int}"] = p =>
                ReturnHomeAction(new RecentBlogPostsBindingModel() { Page = p.page, Take = 10 });

            Get["/tag/{Tag}"] = p =>
                ReturnArticlesTaggedBy(new TaggedBlogPostsBindingModel() { Tag = p.tag });

            Get[@"/(?<year>\d{4})/(?<month>0[1-9]|1[0-2])/(?<titleslug>[a-zA-Z0-9_-]+)"] = p =>
                ReturnArticle(new BlogPostDetailsBindingModel { Permalink = p.titleslug }, spamShield);//TODO:需要增加对日期有效性的验证

            Get[@"/(?<year>\d{4})/(?<month>0[1-9]|1[0-2])"] = p =>
                                                {
                                                    var input = new IntervalBlogPostsBindingModel
                                                    {
                                                        FromDate = new DateTime(p.year, p.month, 1)
                                                    };

                                                    input.ToDate = input.FromDate.AddMonths(1);

                                                    return ReturnArticles(input);
                                                };

            Post["/spam/hash/{tick}"] = p => spamShield.GenerateHash(p.tick);

            Post["/comment/(?<titleslug>[a-zA-Z0-9_-]+)"] = p => ReturnAddComment(p);
        }

        public dynamic ReturnAddComment(dynamic p)
        {
            var postModel = _viewFactory.Get<BlogPostDetailsBindingModel, BlogPostDetailsViewModel>(new BlogPostDetailsBindingModel { Permalink = p.titleslug });
            if (postModel != null)
            {
                var newCommentCommand = this.BindAndValidate<NewCommentCommand>();
                if (!ModelValidationResult.IsValid)
                {
                    return HttpStatusCode.UnprocessableEntity;
                }
                newCommentCommand.SpamShield = this.Bind<SpamShield>();
                newCommentCommand.IPAddress = Request.UserHostAddress;
                newCommentCommand.PostId = postModel.BlogPost.Id;
                var result = _commandInvokerFactory.Handle<NewCommentCommand, CommandResult>(newCommentCommand);
                if (result.Success)
                    return Response.AsRedirect(string.Format("{0}#comment_{1}", postModel.BlogPost.GetLink(), newCommentCommand.Id));
            }
            return HttpStatusCode.NotFound;
        }

        public dynamic ReturnHomeAction(RecentBlogPostsBindingModel input)
        {
            var model = _viewFactory.Get<RecentBlogPostsBindingModel, RecentBlogPostsViewModel>(input);
            if (model.Posts.Count() == 0)
            {
                if (input.Page > 1)
                    return HttpStatusCode.NotFound;
                else
                    return Response.AsText("MZBlog尚未发现任何已经发布的文章哦!", "text/html; charset=utf-8");
            }
            if (input.Page == 1)
                ViewBag.Title = "首页";
            else
                ViewBag.Title = "文章列表";
            
            return View["Index", model];
        }

        public dynamic ReturnArticle(BlogPostDetailsBindingModel input, ISpamShieldService spamShield)
        {
            var model = _viewFactory.Get<BlogPostDetailsBindingModel, BlogPostDetailsViewModel>(input);

            if (model == null)
                return HttpStatusCode.NotFound;

            ViewBag.Title = model.BlogPost.Title;

            ViewBag.Tick = spamShield.CreateTick(input.Permalink);

            return View["details", model];
        }

        public Negotiator ReturnArticles(IntervalBlogPostsBindingModel input)
        {
            var model = _viewFactory.Get<IntervalBlogPostsBindingModel, IntervalBlogPostsViewModel>(input);

            return View["Archive", model];
        }

        public dynamic ReturnArticlesTaggedBy(TaggedBlogPostsBindingModel input)
        {
            var model = _viewFactory.Get<TaggedBlogPostsBindingModel, TaggedBlogPostsViewModel>(input);
            if (model == null)
                return HttpStatusCode.NotFound;

            ViewBag.Title = "标签:" + model.Tag;

            return View["Tagged", model];
        }
    }
}