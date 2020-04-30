
CREATE TABLE Author (
    Id varchar(24) PRIMARY KEY NOT NULL,
    Email varchar(256) NOT NULL,
    HashedPassword varchar(200) NOT NULL,
    DisplayName nvarchar(50) NOT NULL,
    CreatedUTC datetime NOT NULL
);

-- add user (account:mz@bl.og password:mzblog)
INSERT INTO Author (Id,Email,HashedPassword,DisplayName,CreatedUTC) VAlUES ('5e93ba8449cd35acc84dbc34','mz@bl.og','764be383cb0aec94f47d9cd5b34d9c95','mzblog',DATETIME('now'));

CREATE TABLE BlogPost (
    Id varchar(24) PRIMARY KEY NOT NULL,
    Title nvarchar(100) NOT NULL,
    TitleSlug varchar(200) NOT NULL,
    ViewCount int NOT NULL DEFAULT 0,
    MarkDown text,
    Content text,
    [Status] int,
    PublishUTC datetime NOT NULL,
    CreatedUTC datetime NOT NULL,
    AuthorDisplayName nvarchar(100) NOT NULL,
    AuthorEmail varchar(100) NOT NULL
);

CREATE INDEX ix_BlogPost_PublishUTC On BlogPost ([Status],PublishUTC);
CREATE INDEX ix_BlogPost_TitleSlug On BlogPost (TitleSlug);

CREATE TABLE Tag (
    Slug varchar(50) PRIMARY KEY NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    PostCount int NOT NULL DEFAULT 0
);

CREATE TABLE BlogPostTags (
    BlogPostId varchar(24) NOT NULL,
    TagSlug varchar(50) NOT NULL,
    PRIMARY KEY (BlogPostId,TagSlug)
);

CREATE TABLE BlogComment (
    Id varchar(24) PRIMARY KEY NOT NULL,
    Content text NOT NULL,
    NickName nvarchar(50) NOT NULL,
    Email varchar(256) NULL,
    SiteUrl varchar(2000),
    CreatedTime datetime NOT NULL,
    PostId varchar(24) NOT NULL,
    IPAddress varchar(100)
);

CREATE INDEX ix_BlogComment_PostId On BlogComment (PostId,CreatedTime);

CREATE TABLE SpamHash (
    Id varchar(24) PRIMARY KEY NOT NULL,
    PostKey varchar(200) NOT NULL,
    [Hash] varchar(100),
    Pass varchar(100),
    CreatedTime datetime NOT NULL
);

CREATE TABLE VisitIp (
    Ip varchar(100) PRIMARY KEY NOT NULL,
    Country varchar(100),
    Region varchar(100),
    City varchar(100),
    VisitCount int NOT NULL DEFAULT 1,
    FirstVisitTime datetime NOT NULL,
    LastVisitTime datetime
);