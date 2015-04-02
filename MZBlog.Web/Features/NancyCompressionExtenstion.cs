using Nancy;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;

namespace MZBlog.Web.Features
{
    //from url http://simoncropp.com/httpcompressionwithnancyfx
    public static class NancyCompressionExtenstion
    {
        public static void CheckForCompression(NancyContext context)
        {
            if (!RequestIsGzipCompatible(context.Request))
            {
                return;
            }

            if (context.Response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }

            if (!ResponseIsCompatibleMimeType(context.Response))
            {
                return;
            }

            if (ContentLengthIsTooSmall(context.Response))
            {
                return;
            }

            CompressResponse(context.Response);
        }

        private static void CompressResponse(Response response)
        {
            response.Headers["Content-Encoding"] = "gzip";

            var contents = response.Contents;
            response.Contents = responseStream =>
                                    {
                                        using (var compression = new GZipStream(responseStream, CompressionMode.Compress))
                                        {
                                            contents(compression);
                                        }
                                    };
        }

        private static bool ContentLengthIsTooSmall(Response response)
        {
            string contentLength;
            if (response.Headers.TryGetValue("Content-Length", out contentLength))
            {
                var length = long.Parse(contentLength);
                if (length < 4096)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<string> ValidMimes = new List<string>
                                                {
                                                    "text/css",
                                                    "text/html",
                                                    "text/plain",
                                                    "application/xml",
                                                    "application/json",
                                                    "application/xaml+xml",
                                                    "application/x-javascript"
                                                };

        private static bool ResponseIsCompatibleMimeType(Response response)
        {
            return ValidMimes.Any(x => x == response.ContentType);
        }

        private static bool RequestIsGzipCompatible(Request request)
        {
            return request.Headers.AcceptEncoding.Any(x => x.Contains("gzip"));
        }
    }
}