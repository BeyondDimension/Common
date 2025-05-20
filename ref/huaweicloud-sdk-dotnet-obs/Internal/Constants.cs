/*----------------------------------------------------------------------------------
// Copyright 2019 Huawei Technologies Co.,Ltd.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License.  You may obtain a copy of the
// License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations under the License.
//----------------------------------------------------------------------------------*/
using System.Globalization;

namespace OBS.Internal
{
    internal static class Constants
    {

        internal static class CommonHeaders
        {
            public const string Connection = "Connection";

            public const string Range = "Range";

            public const string LastModified = "Last-Modified";

            public const string Location = "Location";

            public const string Expires = "Expires";

            public const string Date = "Date";

            public const string ContentMd5 = "Content-MD5";

            public const string ContentLength = "Content-Length";

            public const string ContentEncoding = "Content-Encoding";

            public const string ContentDisposition = "Content-Disposition";

            public const string ContentType = "Content-Type";

            public const string ETag = "ETag";

            public const string CacheControl = "Cache-Control";

            public const string Authorization = "Authorization";

            public const string Host = "Host";

            public const string IfModifiedSince = "If-Modified-Since";

            public const string IfUnmodifiedSince = "If-Unmodified-Since";

            public const string IfMatch = "If-Match";

            public const string IfNoneMatch = "If-None-Match";

            public const string UserAgent = "User-Agent";

            public const string OriginHeader = "Origin";

            public const string AccessControlRequestHeader = "Access-Control-Request-Headers";

        }

        internal static class ObsRequestParams
        {
            public const string UploadId = "uploadId";
            public const string PartNumber = "partNumber";
            public const string Prefix = "prefix";
            public const string Delimiter = "delimiter";
            public const string Marker = "marker";
            public const string KeyMarker = "key-marker";
            public const string MaxKeys = "max-keys";
            public const string VersionIdMarker = "version-id-marker";
            public const string MaxUploads = "max-uploads";
            public const string UploadIdMarker = "upload-id-marker";
            public const string VersionId = "versionId";
            public const string MaxParts = "max-parts";
            public const string PartNumberMarker = "part-number-marker";
            public const string ImageProcess = "x-image-process";
            public const string ResponseContentType = "response-content-type";
            public const string ResponseContentLanguage = "response-content-language";
            public const string ResponseExpires = "response-expires";
            public const string ResponseCacheControl = "response-cache-control";
            public const string ResponseContentDisposition = "response-content-disposition";
            public const string ResponseContentEncoding = "response-content-encoding";
            public const string Position = "position";

        }

        public const string UrlEncodedContent = "application/x-www-form-urlencoded; charset=utf-8";

        public const string ISO8601DateFormat = "yyyy-MM-dd\\THH:mm:ss.fff\\Z";

        public const string ISO8601DateFormatMidNight = "yyyy-MM-dd\\T00:00:00\\Z";

        public const string ISO8601DateFormatNoMS = "yyyy-MM-dd\\THH:mm:ss\\Z";

        public const string LongDateFormat = "yyyyMMddTHHmmssZ";

        public const string ShortDateFormat = "yyyyMMdd";

        public const string RFC822DateFormat = "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T";

        public const string SubResourceApiVersion = "apiversion";

        public const int DefaultBufferSize = 8192;

        public const long DefaultProgressUpdateInterval = 102400;

        public const int DefaultMaxIdleTime = 30 * 1000;
        public const int DefaultReadWriteTimeout = 60 * 1000;
        public const int DefaultMaxErrorRetry = 3;
        public const int DefaultConnectTimeout = -1;
        public const int DefaultAsyncSocketTimeout = -1;
        public const int DefaultConnectionLimit = 1000;
        public const bool DefaultKeepAlive = true;
        public const bool DefaultAuthTypeNegotiation = true;

        public const AuthTypeEnum DefaultAuthType = AuthTypeEnum.OBS;

        public const string ObsHeaderPrefix = "x-obs-";

        public const string V2HeaderPrefix = "x-amz-";

        public const string ObsHeaderMetaPrefix = "x-obs-meta-";

        public const string V2HeaderMetaPrefix = "x-amz-meta-";

        public const string ObsSdkVersion = "3.20.7";

        public const string ObsApiHeader = "api";
        public const string ObsApiHeaderWithPrefix = ObsHeaderPrefix + ObsApiHeader;

        public const string SdkUserAgent = "obs-sdk-.net/" + Constants.ObsSdkVersion;

        public const string NullRequest = "NullRequest";
        public const string NullRequestMessage = "request is null";

        public const string InvalidBucketName = "InvalidBucketName";
        public const string InvalidBucketNameMessage = "bucket name is not valid";

        public const string InvalidObjectKey = "InvalidObjectKey";
        public const string InvalidObjectKeyMessage = "object key is null";

        public const string InvalidSourceBucketNameMessage = "source object key is null";
        public const string InvalidSourceObjectKeyMessage = "source bucket name is null";

        public const string InvalidUploadId = "InvalidUploadId";
        public const string InvalidUploadIdMessage = "upload id is not valid";

        public const string InvalidPartNumber = "InvalidPartNumber";
        public const string InvalidPartNumberMessage = "part number is not valid";

        public const string InvalidEndpoint = "InvalidEndpoint";
        public const string InvalidEndpointMessage = "endpoint is not valid";

        public const string DefaultEncoding = "utf-8";

        public const long DefaultStreamBufferThreshold = 0;

        public static readonly CultureInfo CultureInfo = CultureInfo.GetCultureInfo("en-US");

        public const string RequestTimeout = "RequestTimeout";

        public const string AllowedInUrl = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~:'()!*";

#if NET9_0_OR_GREATER
        private static readonly global::System.Threading.Lock _lock = new();
#else
        private static readonly object _lock = new();
#endif

        private static volatile IList<string>? _AllowedResponseHttpHeaders;

        public const string ThreeAz = "3az";

        public const string ObsHeadErrorCode = "x-obs-error-code";

        public const string ObsHeadErrorMessage = "x-obs-error-message";

        public static IList<string> AllowedResponseHttpHeaders
        {
            get
            {
                if (_AllowedResponseHttpHeaders == null)
                {
                    lock (_lock)
                    {
                        if (_AllowedResponseHttpHeaders == null)
                        {
                            IList<string> tempAllowedResponseHttpHeaders =
                            [
                                "content-type",
                                "content-md5",
                                "content-length",
                                "content-language",
                                "expires",
                                "origin",
                                "cache-control",
                                "content-disposition",
                                "content-encoding",
                                "x-default-storage-class",
                                "location",
                                "date",
                                "etag",
                                "host",
                                "last-modified",
                                "content-range",
                                "x-reserved",
                                "access-control-allow-origin",
                                "access-control-allow-headers",
                                "access-control-max-age",
                                "access-control-allow-methods",
                                "access-control-expose-headers",
                                "connection",
                            ];
                            _AllowedResponseHttpHeaders = tempAllowedResponseHttpHeaders;
                        }
                    }
                }
                return _AllowedResponseHttpHeaders;
            }

        }

        private static volatile IList<string>? _AllowedRequestHttpHeaders;

        public static IList<string> AllowedRequestHttpHeaders
        {
            get
            {
                if (_AllowedRequestHttpHeaders == null)
                {
                    lock (_lock)
                    {
                        if (_AllowedRequestHttpHeaders == null)
                        {
                            IList<string> tempAllowedRequestHttpHeaders =
                            [
                                "content-type",
                                "content-md5",
                                "content-length",
                                "content-language",
                                "expires",
                                "origin",
                                "cache-control",
                                "content-disposition",
                                "content-encoding",
                                "access-control-request-method",
                                "access-control-request-headers",
                                "success-action-redirect",
                                "x-default-storage-class",
                                "location",
                                "date",
                                "etag",
                                "range",
                                "host",
                                "if-modified-since",
                                "if-unmodified-since",
                                "if-match",
                                "if-none-match",
                                "last-modified",
                                "content-range",
                            ];
                            _AllowedRequestHttpHeaders = tempAllowedRequestHttpHeaders;
                        }
                    }
                }
                return _AllowedRequestHttpHeaders;
            }

        }

        private static volatile IList<string>? _AllowedResourceParameters;

        public static IList<string> AllowedResourceParameters
        {
            get
            {

                if (_AllowedResourceParameters == null)
                {
                    lock (_lock)
                    {
                        if (_AllowedResourceParameters == null)
                        {
                            IList<string> tempAllowedResourceParameters =
                            [
                                "acl",
                                "backtosource",
                                "policy",
                                "torrent",
                                "logging",
                                "location",
                                "storageinfo",
                                "quota",
                                "storagepolicy",
                                "storageclass",
                                "requestpayment",
                                "versions",
                                "versioning",
                                "versionid",
                                "uploads",
                                "uploadid",
                                "partnumber",
                                "website",
                                "notification",
                                "lifecycle",
                                "delete",
                                "cors",
                                "restore",
                                "tagging",
                                "append",
                                "position",
                                "replication",
                                "response-content-type",
                                "response-content-language",
                                "response-expires",
                                "response-cache-control",
                                "response-content-disposition",
                                "response-content-encoding",
                                "x-image-process",
                                "x-oss-process",
                            ];
                            _AllowedResourceParameters = tempAllowedResourceParameters;
                        }
                    }
                }
                return _AllowedResourceParameters;
            }

        }

        public static volatile IDictionary<string, string>? _MimeTypes;


        public static IDictionary<string, string> MimeTypes
        {
            get
            {
                if (_MimeTypes == null)
                {
                    lock (_lock)
                    {
                        if (_MimeTypes == null)
                        {
                            IDictionary<string, string> tempMimeTypes = new Dictionary<string, string>
                            {
                                { "7z", "application/x-7z-compressed" },
                                { "aac", "audio/x-aac" },
                                { "ai", "application/postscript" },
                                { "aif", "audio/x-aiff" },
                                { "asc", "text/plain" },
                                { "asf", "video/x-ms-asf" },
                                { "atom", "application/atom+xml" },
                                { "avi", "video/x-msvideo" },
                                { "bmp", "image/bmp" },
                                { "bz2", "application/x-bzip2" },
                                { "cer", "application/pkix-cert" },
                                { "crl", "application/pkix-crl" },
                                { "crt", "application/x-x509-ca-cert" },
                                { "css", "text/css" },
                                { "csv", "text/csv" },
                                { "cu", "application/cu-seeme" },
                                { "deb", "application/x-debian-package" },
                                { "doc", "application/msword" },
                                { "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                                { "dvi", "application/x-dvi" },
                                { "eot", "application/vnd.ms-fontobject" },
                                { "eps", "application/postscript" },
                                { "epub", "application/epub+zip" },
                                { "etx", "text/x-setext" },
                                { "flac", "audio/flac" },
                                { "flv", "video/x-flv" },
                                { "gif", "image/gif" },
                                { "gz", "application/gzip" },
                                { "htm", "text/html" },
                                { "html", "text/html" },
                                { "ico", "image/x-icon" },
                                { "ics", "text/calendar" },
                                { "ini", "text/plain" },
                                { "iso", "application/x-iso9660-image" },
                                { "jar", "application/java-archive" },
                                { "jpe", "image/jpeg" },
                                { "jpeg", "image/jpeg" },
                                { "jpg", "image/jpeg" },
                                { "js", "text/javascript" },
                                { "json", "application/json" },
                                { "latex", "application/x-latex" },
                                { "log", "text/plain" },
                                { "m4a", "audio/mp4" },
                                { "m4v", "video/mp4" },
                                { "mid", "audio/midi" },
                                { "midi", "audio/midi" },
                                { "mov", "video/quicktime" },
                                { "mp3", "audio/mpeg" },
                                { "mp4", "video/mp4" },
                                { "mp4a", "audio/mp4" },
                                { "mp4v", "video/mp4" },
                                { "mpe", "video/mpeg" },
                                { "mpeg", "video/mpeg" },
                                { "mpg", "video/mpeg" },
                                { "mpg4", "video/mp4" },
                                { "oga", "audio/ogg" },
                                { "ogg", "audio/ogg" },
                                { "ogv", "video/ogg" },
                                { "ogx", "application/ogg" },
                                { "pbm", "image/x-portable-bitmap" },
                                { "pdf", "application/pdf" },
                                { "pgm", "image/x-portable-graymap" },
                                { "png", "image/png" },
                                { "pnm", "image/x-portable-anymap" },
                                { "ppm", "image/x-portable-pixmap" },
                                { "ppt", "application/vnd.ms-powerpoint" },
                                { "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                                { "ps", "application/postscript" },
                                { "qt", "video/quicktime" },
                                { "rar", "application/x-rar-compressed" },
                                { "ras", "image/x-cmu-raster" },
                                { "rss", "application/rss+xml" },
                                { "rtf", "application/rtf" },
                                { "sgm", "text/sgml" },
                                { "sgml", "text/sgml" },
                                { "svg", "image/svg+xml" },
                                { "swf", "application/x-shockwave-flash" },
                                { "tar", "application/x-tar" },
                                { "tif", "image/tiff" },
                                { "tiff", "image/tiff" },
                                { "torrent", "application/x-bittorrent" },
                                { "ttf", "application/x-font-ttf" },
                                { "txt", "text/plain" },
                                { "wav", "audio/x-wav" },
                                { "webm", "video/webm" },
                                { "wma", "audio/x-ms-wma" },
                                { "wmv", "video/x-ms-wmv" },
                                { "woff", "application/x-font-woff" },
                                { "wsdl", "application/wsdl+xml" },
                                { "xbm", "image/x-xbitmap" },
                                { "xls", "application/vnd.ms-excel" },
                                { "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                                { "xml", "application/xml" },
                                { "xpm", "image/x-xpixmap" },
                                { "xwd", "image/x-xwindowdump" },
                                { "yaml", "text/yaml" },
                                { "yml", "text/yaml" },
                                { "zip", "application/zip" }
                            };
                            _MimeTypes = tempMimeTypes;
                        }
                    }
                }
                return _MimeTypes;
            }

        }

    }
}
