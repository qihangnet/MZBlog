using MongoDB.Driver;
using MZBlog.Core;
using MZBlog.Core.Cache;
using MZBlog.Web.Features;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using System;
using System.Configuration;
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
            if (ex is MongoDB.Driver.MongoConnectionException)
            {
                return "MongoDB can't connect.";
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

            container.Register(typeof(MongoDatabase), (cContainer, overloads) => Database);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("scripts"));
        }

        public virtual MongoDatabase Database
        {
            get
            {
                var url = MongoUrl.Create(ConfigurationManager.ConnectionStrings["MZBlogDB"].ConnectionString);
                var client = new MongoClient(url);
                var server = client.GetServer();
                return server.GetDatabase(url.DatabaseName);
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