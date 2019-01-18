using System;
using System.IO;
using System.Linq;
using iBoxDB.LocalServer;
using MZBlog.Core;
using Dapper;
using Dapper.Extensions;
using Microsoft.Data.Sqlite;
using Mapster;
using MZBlog.Core.Entities;

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
            Console.WriteLine("清理sqlite数据库 ...");
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
            Console.WriteLine("迁移用户数据 ...");
            conn.MigrateAuthor();
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

        static void MigrateAuthor(this SqliteConnection conn)
        {
            var list = (from t in db.Select<Old.Author>("from " + DBTableNames.Authors)
                        select t);
            foreach (var item in list)
            {
                var author = item.Adapt<Author>();
                var firstPost = (from t in db.Select<Old.BlogPost>("from " + DBTableNames.BlogPosts)
                                 orderby t.DateUTC descending
                                 select t).FirstOrDefault();
                author.CreatedUTC = firstPost == null ? DateTime.UtcNow : firstPost.DateUTC;

                conn.Insert(author);
            }
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
}
