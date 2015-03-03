using iBoxDB.LocalServer;
using MZBlog.Core;
using MZBlog.Core.Cache;
using MZBlog.Core.Documents;
using MZBlog.Web.Features;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using System;
using System.Linq;
using System.Reflection;

namespace MZBlog.Web
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            StaticConfiguration.Caching.EnableRuntimeViewUpdates = true;
            StaticConfiguration.DisableErrorTraces = false;
            pipelines.OnError += ErrorHandler;
        }

        private Response ErrorHandler(NancyContext ctx, Exception ex)
        {
            if (ex is iBoxDB.E.DatabaseShutdownException)
            {
                return "DB can't connect.";
            }
            return null;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register(typeof(ISpamShieldService), typeof(SpamShieldService));
            container.Register(typeof(ICache), typeof(RuntimeCache));

            RegisterIViewProjections(container);
            RegisterICommandInvoker(container);
            container.Register<DB.AutoBox>((c, o) => Database);
            //container.Register(typeof(MongoDatabase), (cContainer, overloads) => Database);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("scripts"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("content"));
        }

        public virtual DB.AutoBox Database
        {
            get
            {
                var server = new DB(@"..\App_Data\ibox");
                server.GetConfig().EnsureTable<Author>(DBTableNames.Authors, "Id")
                    .EnsureIndex<Author>(DBTableNames.Authors, "Email")
                    .EnsureTable<BlogPost>(DBTableNames.BlogPosts, "Id")
                    .EnsureTable<BlogPost>(DBTableNames.BlogPosts, "TitleSlug", "Status", "PubDate", "DateUTC")
                    .EnsureTable<BlogComment>(DBTableNames.BlogComments, "Id")
                    .EnsureTable<BlogComment>(DBTableNames.BlogComments,"PostId")
                    .EnsureTable<SpamHash>(DBTableNames.SpamHashes, "Id")
                    .EnsureTable<Tag>(DBTableNames.Tags, "Slug");

                var db = server.Open();
                return db;
            }
        }

        public static void RegisterICommandInvoker(TinyIoCContainer container)
        {
            var commandInvokerTypes = Assembly.GetAssembly(typeof(ICommandInvoker<,>))
                                              .DefinedTypes
                                              .Select(t => new
                                              {
                                                  Type = t.AsType(),
                                                  Interface = t.ImplementedInterfaces.FirstOrDefault(
                                                      i =>
                                                      i.IsGenericType() &&
                                                      i.GetGenericTypeDefinition() == typeof(ICommandInvoker<,>))
                                              })
                                              .Where(t => t.Interface != null)
                                              .ToArray();

            foreach (var commandInvokerType in commandInvokerTypes)
            {
                container.Register(commandInvokerType.Interface, commandInvokerType.Type);
            }

            container.Register(typeof(ICommandInvokerFactory), (cContainer, overloads) => new CommandInvokerFactory(cContainer));
        }

        public static void RegisterIViewProjections(TinyIoCContainer container)
        {
            var viewProjectionTypes = Assembly.GetAssembly(typeof(IViewProjection<,>))
                                              .DefinedTypes
                                              .Select(t => new
                                                               {
                                                                   Type = t.AsType(),
                                                                   Interface = t.ImplementedInterfaces.FirstOrDefault(
                                                                       i =>
                                                                       i.IsGenericType() &&
                                                                       i.GetGenericTypeDefinition() == typeof(IViewProjection<,>))
                                                               })
                                              .Where(t => t.Interface != null)
                                              .ToArray();

            foreach (var viewProjectionType in viewProjectionTypes)
            {
                container.Register(viewProjectionType.Interface, viewProjectionType.Type);
            }

            container.Register(typeof(IViewProjectionFactory), (cContainer, overloads) => new ViewProjectionFactory(cContainer));
        }
    }
}