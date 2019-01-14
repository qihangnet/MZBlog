using iBoxDB.LocalServer;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;
using MZBlog.Core;
using MZBlog.Core.Documents;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace MZBlog.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // got help from https://www.cnblogs.com/dudu/p/5879913.html
            services.Configure<WebEncoderOptions>(options =>
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin,
                    UnicodeRanges.CjkUnifiedIdeographs));

            services.AddiBoxDB();
            services.AddIpIpDotNet();
            services.AddMediatR();
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvc();
        }
    }

    public static class IServiceCollectionExtensions
    {
        public static void AddiBoxDB(this IServiceCollection services, string dbFolderName = "ibox")
        {
            var dbPath = Path.Combine(Path.GetDirectoryName(typeof(Startup).Assembly.Location), "App_Data", dbFolderName);
            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }

            var server = new DB(dbPath);
            var config = server.GetConfig();

            config.EnsureTable<Author>(DBTableNames.Authors, "Id");
            //config.EnsureIndex<Author>(DBTableNames.Authors, "Email");
            config.EnsureTable<BlogPost>(DBTableNames.BlogPosts, "Id");
            //config.EnsureIndex<BlogPost>(DBTableNames.BlogPosts, "TitleSlug", "Status", "PubDate", "DateUTC");
            config.EnsureTable<BlogComment>(DBTableNames.BlogComments, "Id");
            //config.EnsureIndex<BlogComment>(DBTableNames.BlogComments, "PostId");
            config.EnsureTable<SpamHash>(DBTableNames.SpamHashes, "Id");
            config.EnsureTable<Tag>(DBTableNames.Tags, "Slug");

            var db = server.Open();
            services.AddSingleton(db);

            Core.Extensions.TagExtension.SetupDb(db);
        }

        public static void AddIpIpDotNet(this IServiceCollection services)
        {
            var dbDir = Path.Combine(Path.GetDirectoryName(typeof(Startup).Assembly.Location), "App_Data");
            if (!Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }
            var ipDbPath =Path.Combine(dbDir,"ipipfree.ipdb");
            var ipCity = new IPIP.Net.City(ipDbPath);
            services.AddSingleton(ipCity);
        }
    }
}
