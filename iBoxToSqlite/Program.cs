using System;
using System.IO;
using System.Linq;
using iBoxDB.LocalServer;
using MZBlog.Core;
using Dapper;
using Dapper.Extensions;
using Microsoft.Data.Sqlite;
using Mapster;
using MZBlog.Core.Documents;

namespace iBoxToSqlite
{
    static class Program
    {
        static DB.AutoBox db;
        static IPIP.Net.City ipCity;
        static void Main(string[] args)
        {
            Console.WriteLine("准备IP数据库 ...");
            ipCity = GetIpDb("ipipfree.ipdb");

            Console.WriteLine("准备MZBlog的iBoxDb数据库 ...");
            db = OpeniBoxDb("ibox");

            Console.WriteLine("准备MZBlog的sqlite数据库 ...");
            var conn = GetSqliteConnection("mzblog.db");
            conn.Open();
            // clear all tables
            var list = conn.Query<string>("SELECT name FROM sqlite_master WHERE type='table';");
            foreach (var item in list)
            {
                conn.Execute("DROP TABLE " + item);
            }
            // init all tables
            var sql = System.IO.File.ReadAllText("init-sqlite.sql");
            conn.Execute(sql);

            // begin migrate
            Console.WriteLine("开始迁移MZBlog的数据 ...");
            Console.WriteLine("迁移标签 ...");
            conn.MigrateTag();
            Console.WriteLine("迁移文章 ...");
            conn.MigrateBlogPost();
            Console.WriteLine("迁移评论 ...");
            conn.MigrateBlogComment();
            Console.WriteLine("迁移机器人防护数据 ...");
            conn.MigrateSpamHash();

            // begin VACUUM
            Console.WriteLine("开始优化sqlite数据库 ...");
            conn.Execute("VACUUM");

            conn.Close();

            Console.WriteLine("---======迁移完毕=====---");
        }

        static void MigrateTag(this SqliteConnection conn)
        {
            var list = (from t in db.Select<Old.Tag>("from " + DBTableNames.Tags)
                        select t);
            foreach (var item in list)
            {
                conn.Insert(item.Adapt<Tag>());
            }
        }

        static void MigrateBlogPost(this SqliteConnection conn)
        {
            var list = (from p in db.Select<Old.BlogPost>("from " + DBTableNames.BlogPosts)
                        orderby p.DateUTC
                        select p);
            var tran = conn.BeginTransaction();
            foreach (var p in list)
            {
                var post = p.Adapt<BlogPost>();
                post.PublishUTC = p.DateUTC;
                post.PublishUTC = p.PubDate;

                conn.Insert(post, tran);
                foreach (var t in p.Tags)
                {
                    conn.Insert(new BlogPostTags { BlogPostId = p.Id, TagSlug = t }, tran);
                }
            }
            tran.Commit();
        }

        static void MigrateBlogComment(this SqliteConnection conn)
        {
            var list = (from t in db.Select<Old.BlogComment>("from " + DBTableNames.BlogComments)
                        select t);
            foreach (var item in list)
            {
                var comment = item.Adapt<BlogComment>();
                if (string.IsNullOrWhiteSpace(comment.NickName))
                    comment.NickName = "匿名";
                conn.Insert(comment);
                var ip = comment.IPAddress;
                if (!string.IsNullOrWhiteSpace(ip))
                {
                    var data = conn.Get<VisitIp>(ip);
                    if (data == null)
                    {
                        var cityInfo = ipCity.FindInfo(ip);
                        conn.Insert(new VisitIp
                        {
                            Ip = ip,
                            Country = cityInfo.CountryName,
                            Region = cityInfo.RegionName,
                            City = cityInfo.CityName,
                            FirstVisitTime = comment.CreatedTime,
                            VisitCount = 1
                        });
                    }
                    else
                    {
                        data.LastVisitTime = comment.CreatedTime;
                        data.VisitCount++;
                        conn.Update(data);
                    }

                }
            }
        }

        static void MigrateSpamHash(this SqliteConnection conn)
        {
            var list = (from t in db.Select<Old.SpamHash>("from " + DBTableNames.SpamHashes)
                        select t);
            var tran = conn.BeginTransaction();
            foreach (var item in list)
            {
                conn.Insert(item.Adapt<SpamHash>(), tran);
            }
            tran.Commit();
        }

        static string RuntimeAppDataPath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "App_Data");
            }
        }

        static SqliteConnection GetSqliteConnection(string dbFile)
        {
            var dbPath = Path.Combine(RuntimeAppDataPath, dbFile);
            var connString = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();
            return new SqliteConnection(connString);
        }

        static IPIP.Net.City GetIpDb(string dbFile)
        {
            if (!Directory.Exists(RuntimeAppDataPath))
            {
                Directory.CreateDirectory(RuntimeAppDataPath);
            }
            var ipDbPath = Path.Combine(RuntimeAppDataPath, dbFile);
            var ipCity = new IPIP.Net.City(ipDbPath);
            return ipCity;
        }

        static DB.AutoBox OpeniBoxDb(string dbFolderName)
        {
            var dbPath = Path.Combine(RuntimeAppDataPath, dbFolderName);
            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }

            var server = new DB(dbPath);
            var config = server.GetConfig();

            config.EnsureTable<Old.Author>(DBTableNames.Authors, "Id");
            //config.EnsureIndex<Author>(DBTableNames.Authors, "Email");
            config.EnsureTable<Old.BlogPost>(DBTableNames.BlogPosts, "Id");
            //config.EnsureIndex<BlogPost>(DBTableNames.BlogPosts, "TitleSlug", "Status", "PubDate", "DateUTC");
            config.EnsureTable<Old.BlogComment>(DBTableNames.BlogComments, "Id");
            //config.EnsureIndex<BlogComment>(DBTableNames.BlogComments, "PostId");
            config.EnsureTable<Old.SpamHash>(DBTableNames.SpamHashes, "Id");
            config.EnsureTable<Old.Tag>(DBTableNames.Tags, "Slug");

            var db = server.Open();
            return db;
        }
    }

    // public class BlogPost
    // {
    //     [ExplicitKey]
    //     public string Id { get; set; }

    //     public string Title { get; set; }

    //     public string TitleSlug { get; set; }

    //     public int ViewCount { get; set; }

    //     public string MarkDown { get; set; }

    //     public string Content { get; set; }

    //     public Old.PublishStatus Status { get; set; }

    //     public DateTime PublishUTC { get; set; }

    //     public DateTime CreatedUTC { get; set; }

    //     // public string[] Tags { get; set; }

    //     public string AuthorDisplayName { get; set; }

    //     public string AuthorEmail { get; set; }
    // }

    // public class Tag
    // {
    //     [ExplicitKey]

    //     public string Slug { get; set; }

    //     public string Name { get; set; }

    //     public int PostCount { get; set; }
    // }

    // public class BlogPostTags
    // {
    //     [ExplicitKey]
    //     public string BlogPostId { get; set; }

    //     [ExplicitKey]
    //     public string TagSlug { get; set; }
    // }

    // public class BlogComment
    // {
    //     [ExplicitKey]
    //     public string Id { get; set; }

    //     public string Content { get; set; }

    //     public string NickName { get; set; }

    //     public string Email { get; set; }

    //     public string SiteUrl { get; set; }

    //     public DateTime CreatedTime { get; set; }

    //     public string PostId { get; set; }

    //     public string IPAddress { get; set; }
    // }

    // public class SpamHash
    // {
    //     [ExplicitKey]
    //     public string Id { get; set; }

    //     public string PostKey { get; set; }

    //     public string Hash { get; set; }

    //     public bool Pass { get; set; }

    //     public DateTime CreatedTime { get; set; }
    // }

    // public class VisitIp
    // {
    //     [ExplicitKey]
    //     public string Ip { get; set; }
    //     public string Country { get; set; }
    //     public string Region { get; set; }
    //     public string City { get; set; }
    //     public int VisitCount { get; set; }
    //     public DateTime FirstVisitTime { get; set; }
    //     public DateTime? LastVisitTime { get; set; }
    // }
}
