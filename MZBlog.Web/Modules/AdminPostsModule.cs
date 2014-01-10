using MZBlog.Core;
using MZBlog.Core.Commands.Posts;
using MZBlog.Core.Extensions;
using MZBlog.Core.ViewProjections.Admin;
using MZBlog.Core.ViewProjections.Home;
using Nancy;
using Nancy.Extensions;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;

namespace MZBlog.Web.Modules
{
    public class AdminPostsModule : SecureModule
    {
        private readonly ICommandInvokerFactory _commandInvokerFactory;

        public AdminPostsModule(IViewProjectionFactory factory, ICommandInvokerFactory commandInvokerFactory)
            : base(factory)
        {
            _commandInvokerFactory = commandInvokerFactory;
            Get["/mz-admin/posts/{page?1}"] = _ => ShowPosts(_.page);
            Get["/mz-admin/posts/new"] = _ => ShowNewPost();
            Post["/mz-admin/posts/new"] = _ =>
                                           {
                                               var command = this.Bind<NewPostCommand>();
                                               command.Author = CurrentUser;
                                               return CreateNewPost(command);
                                           };
            Get["/mz-admin/posts/edit/{postId}"] = _ => ShowPostEdit(_.postId);
            Post["/mz-admin/posts/edit/{postid}"] = _ => EditPost(this.Bind<EditPostCommand>());
            Get["/mz-admin/posts/delete/{postid}"] = _ => DeletePost(this.Bind<DeletePostCommand>());

            Get["/mz-admin/comments/{page?1}"] = _ => ShowComments(_.page);
            Get["/mz-admin/comments/delete/{commentid}"] = _ => DeleteComment(this.Bind<DeleteCommentCommand>());
            Get["/mz-admin/tags"] = _ => ShowTags();
            Get["/mz-admin/slug"] = _ => GetSlug(Request.Query.w);
        }

        private string GetSlug(string words)
        {
            return words.ToSlug();
        }

        private dynamic ShowTags()
        {
            var tags = _viewProjectionFactory.Get<TagCloudBindingModel, TagCloudViewModel>(new TagCloudBindingModel() { Threshold = 1 });
            return View["Tags", tags];
        }

        private dynamic DeletePost(DeletePostCommand deletePostCommand)
        {
            _commandInvokerFactory.Handle<DeletePostCommand, CommandResult>(deletePostCommand);
            string returnURL = Request.Headers.Referrer;
            return Response.AsRedirect(returnURL);
        }

        private dynamic ShowNewPost()
        {
            return View["New", new NewPostCommand()];
        }

        private dynamic CreateNewPost(NewPostCommand command)
        {
            var commandResult = _commandInvokerFactory.Handle<NewPostCommand, CommandResult>(command);

            if (commandResult.Success)
            {
                return this.Context.GetRedirect("/mz-admin/posts");
            }

            AddMessage("保存文章时发生错误", "warning");

            return View["New", command];
        }

        private Negotiator EditPost(EditPostCommand command)
        {
            var commandResult = _commandInvokerFactory.Handle<EditPostCommand, CommandResult>(command);

            if (commandResult.Success)
            {
                AddMessage("文章更新成功", "success");

                return ShowPostEdit(command.PostId);
            }

            return View["Edit", commandResult.GetErrors()];
        }

        private Negotiator ShowPosts(int page)
        {
            var model =
                _viewProjectionFactory.Get<AllBlogPostsBindingModel, AllBlogPostsViewModel>(new AllBlogPostsBindingModel()
                {
                    Page = page,
                    Take = 30
                });
            return View["Posts", model];
        }

        private Negotiator ShowPostEdit(string blogPostId)
        {
            var model =
                _viewProjectionFactory.Get<BlogPostEditBindingModel, BlogPostEditViewModel>(
                    new BlogPostEditBindingModel
                    {
                        PostId = blogPostId
                    }
                );

            return View["Edit", model];
        }

        private Negotiator ShowComments(int page)
        {
            var model =
                    _viewProjectionFactory.Get<AllBlogCommentsBindingModel, AllBlogCommentsViewModel>(new AllBlogCommentsBindingModel()
                    {
                        Page = page
                    });
            return View["Comments", model];
        }

        private dynamic DeleteComment(DeleteCommentCommand deletePostCommand)
        {
            _commandInvokerFactory.Handle<DeleteCommentCommand, CommandResult>(deletePostCommand);
            string returnURL = Request.Headers.Referrer;
            return Response.AsRedirect(returnURL);
        }
    }
}