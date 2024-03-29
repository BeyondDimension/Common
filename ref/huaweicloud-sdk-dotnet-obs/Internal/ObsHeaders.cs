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
using System;
using System.Collections.Generic;
using System.Text;

namespace OBS.Internal
{
    internal class ObsHeaders : IHeaders
    {

        private static ObsHeaders instance = new ObsHeaders();

        private ObsHeaders()
        {

        }

        public static IHeaders GetInstance()
        {
            return instance;
        }

        public string AclHeader()
        {
            return HeaderPrefix() + "acl";
        }

        public string AzRedundancyHeader()
        {
            return HeaderPrefix() + "az-redundancy";
        }

        public string BucketRegionHeader()
        {
            return HeaderPrefix() + "bucket-location";
        }

        public string ContentSha256Header()
        {
            return null;
        }

        public string CopySourceHeader()
        {
            return HeaderPrefix() + "copy-source";
        }

        public string CopySourceIfMatchHeader()
        {
            return HeaderPrefix() + "copy-source-if-match";
        }

        public string CopySourceIfModifiedSinceHeader()
        {
            return HeaderPrefix() + "copy-source-if-modified-since";
        }

        public string CopySourceIfNoneMatchHeader()
        {
            return HeaderPrefix() + "copy-source-if-none-match";
        }

        public string CopySourceIfUnmodifiedSinceHeader()
        {
            return HeaderPrefix() + "copy-source-if-unmodified-since";
        }

        public string CopySourceRangeHeader()
        {
            return HeaderPrefix() + "copy-source-range";
        }

        public string CopySourceSseCHeader()
        {
            return HeaderPrefix() + "copy-source-server-side-encryption-customer-algorithm";
        }

        public string CopySourceSseCKeyHeader()
        {
            return HeaderPrefix() + "copy-source-server-side-encryption-customer-key";
        }

        public string CopySourceSseCKeyMd5Header()
        {
            return HeaderPrefix() + "copy-source-server-side-encryption-customer-key-MD5";
        }

        public string CopySourceVersionIdHeader()
        {
            return HeaderPrefix() + "copy-source-version-id";
        }

        public string DateHeader()
        {
            return HeaderPrefix() + "date";
        }

        public string DefaultStorageClassHeader()
        {
            return HeaderPrefix() + "storage-class";
        }

        public string DeleteMarkerHeader()
        {
            return HeaderPrefix() + "delete-marker";
        }

        public string ExpirationHeader()
        {
            return HeaderPrefix() + "expiration";
        }

        public string ExpiresHeader()
        {
            return HeaderPrefix() + "expires";
        }

        public string GrantFullControlDeliveredHeader()
        {
            return HeaderPrefix() + "grant-full-control-delivered";
        }

        public string GrantFullControlHeader()
        {
            return HeaderPrefix() + "grant-full-control";
        }

        public string GrantReadAcpHeader()
        {
            return HeaderPrefix() + "grant-read-acp";
        }

        public string GrantReadDeliveredHeader()
        {
            return HeaderPrefix() + "grant-read-delivered";
        }

        public string GrantReadHeader()
        {
            return HeaderPrefix() + "grant-read";
        }

        public string GrantWriteAcpHeader()
        {
            return HeaderPrefix() + "grant-write-acp";
        }

        public string GrantWriteHeader()
        {
            return HeaderPrefix() + "grant-write";
        }

        public string HeaderMetaPrefix()
        {
            return Constants.ObsHeaderMetaPrefix;
        }

        public string HeaderPrefix()
        {
            return Constants.ObsHeaderPrefix;
        }

        public string LocationHeader()
        {
            return null;
        }

        public string MetadataDirectiveHeader()
        {
            return HeaderPrefix() + "metadata-directive";
        }

        public string NextPositionHeader()
        {
            return HeaderPrefix() + "next-append-position";
        }

        public string ObjectTypeHeader()
        {
            return HeaderPrefix() + "object-type";
        }

        public string RequestId2Header()
        {
            return HeaderPrefix() + "id-2";
        }

        public string RequestIdHeader()
        {
            return HeaderPrefix() + "request-id";
        }

        public string RestoreHeader()
        {
            return HeaderPrefix() + "restore";
        }

        public string SecurityTokenHeader()
        {
            return HeaderPrefix() + "security-token";
        }

        public string ServerVersionHeader()
        {
            return HeaderPrefix() + "version";
        }

        public string SseCHeader()
        {
            return HeaderPrefix() + "server-side-encryption-customer-algorithm";
        }

        public string SseCKeyHeader()
        {
            return HeaderPrefix() + "server-side-encryption-customer-key";
        }

        public string SseCKeyMd5Header()
        {
            return HeaderPrefix() + "server-side-encryption-customer-key-MD5";
        }

        public string SseKmsHeader()
        {
            return HeaderPrefix() + "server-side-encryption";
        }

        public string SseKmsKeyHeader()
        {
            return HeaderPrefix() + "server-side-encryption-kms-key-id";
        }

        public string StorageClassHeader()
        {
            return HeaderPrefix() + "storage-class";
        }

        public string SuccessRedirectLocationHeader()
        {
            return "success-action-redirect";
        }

        public string VersionIdHeader()
        {
            return HeaderPrefix() + "version-id";
        }

        public string WebsiteRedirectLocationHeader()
        {
            return HeaderPrefix() + "website-redirect-location";
        }

    }
}
