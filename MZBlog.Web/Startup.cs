using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WebEncoders;
using MZBlog.Core;
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
            services.AddSqliteDb();
            services.AddIpIpDotNet();
            services.AddMediatR(typeof(ObjectId).Assembly);
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }

    public static class IServiceCollectionExtensions
    {
        static string RuntimeAppDataPath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "App_Data");
            }
        }

        public static void AddSqliteDb(this IServiceCollection services, string dbFile = "mzblog.db")
        {
            var dbPath = Path.Combine(RuntimeAppDataPath, dbFile);
            var connString = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();
            services.AddTransient(typeof(SqliteConnection), _ => new SqliteConnection(connString));
        }

        public static void AddIpIpDotNet(this IServiceCollection services)
        {
            var dbDir = Path.Combine(RuntimeAppDataPath);
            if (!Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }
            var ipDbPath = Path.Combine(dbDir, "ipipfree.ipdb");
            var ipCity = new IPIP.Net.City(ipDbPath);
            services.AddSingleton(ipCity);
        }
    }
}