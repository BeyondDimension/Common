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
using OBS.Internal;
using OBS.Internal.Auth;
using OBS.Model;

namespace OBS;

#pragma warning disable CS0612 // 类型或成员已过时
#pragma warning disable SA1312 // Variable names should begin with lower-case letter

public partial class ObsClient
{
    /// <summary>
    /// Generate parameters for a temporary authentication request.
    /// </summary>
    /// <param name="request">Request parameters</param>
    /// <returns>Response</returns>
    public CreateTemporarySignatureResponse CreateTemporarySignature(CreateTemporarySignatureRequest request)
    {
        var httpRequest = new HttpRequest
        {
            PathStyle = ObsConfig.PathStyle,
            BucketName = request.BucketName,
            ObjectKey = request.ObjectKey,
            Method = request.Method,
        };

        IHeaders iheaders = httpClient.GetIHeaders(new HttpContext(sp, ObsConfig));

        if (!string.IsNullOrEmpty(sp.Token) && !request.Parameters.ContainsKey(iheaders.SecurityTokenHeader()))
        {
            request.Parameters.Add(iheaders.SecurityTokenHeader(), sp.Token.Trim());
        }

        foreach (KeyValuePair<string, string> entry in request.Headers)
        {
            CommonUtil.AddHeader(httpRequest, entry.Key, entry.Value);
        }

        foreach (KeyValuePair<string, string> entry in request.Parameters)
        {
            CommonUtil.AddParam(httpRequest, entry.Key, entry.Value);
        }

        if (request.SubResource.HasValue)
        {
            SubResourceEnum value = request.SubResource.Value;
            if (value == SubResourceEnum.StoragePolicy && ObsConfig.AuthType == AuthTypeEnum.OBS)
            {
                value = SubResourceEnum.StorageClass;
            }
            else if (value == SubResourceEnum.StorageClass && ObsConfig.AuthType != AuthTypeEnum.OBS)
            {
                value = SubResourceEnum.StoragePolicy;
            }

            CommonUtil.AddParam(httpRequest, EnumAdaptor.GetStringValue(value), null);
        }

        foreach (KeyValuePair<string, string> entry in request.Metadata.KeyValuePairs)
        {
            if (string.IsNullOrEmpty(entry.Key))
            {
                continue;
            }
            var _key = entry.Key;
            if (!entry.Key.StartsWith(iheaders.HeaderMetaPrefix(), StringComparison.OrdinalIgnoreCase) && !entry.Key.StartsWith(Constants.ObsHeaderMetaPrefix, StringComparison.OrdinalIgnoreCase))
            {
                _key = iheaders.HeaderMetaPrefix() + _key;
            }
            CommonUtil.AddHeader(httpRequest, _key, entry.Value);
        }

        long expires = 300;
        if (request.Expires.HasValue && request.Expires.Value > 0)
        {
            expires = request.Expires.Value;
        }

        return ObsConfig.AuthType == AuthTypeEnum.V4 ? CreateV4TemporarySignature(httpRequest, expires, iheaders) : CreateTemporarySignature(httpRequest, expires, iheaders);
    }

    private CreateTemporarySignatureResponse CreateV4TemporarySignature(HttpRequest httpRequest, long expires, IHeaders iheaders)
    {
        var dateDict = V4Signer.GetLongDateAndShortDate(httpRequest, iheaders);
        string host = httpRequest.GetHost(ObsConfig.Endpoint);
        CommonUtil.AddHeader(httpRequest, Constants.CommonHeaders.Host, host);

        var tempDict = new Dictionary<string, string>();

        foreach (KeyValuePair<string, string> entry in httpRequest.Headers)
        {
            if (string.IsNullOrEmpty(entry.Key))
            {
                continue;
            }
            tempDict.Add(entry.Key.Trim().ToLower(), entry.Value);
        }

        List<string> signedHeadersList = V4Signer.GetSignedHeaderList(tempDict);
        string signedHeaders = V4Signer.GetSignedHeaders(signedHeadersList);

        CommonUtil.AddParam(httpRequest, "X-Amz-Algorithm", V4Signer.Algorithm);
        CommonUtil.AddParam(httpRequest, "X-Amz-Credential", sp.Ak + "/" + dateDict["ShortDate"] + V4Signer.ScopeSuffix);
        CommonUtil.AddParam(httpRequest, "X-Amz-Date", dateDict["LongDate"]);
        CommonUtil.AddParam(httpRequest, "X-Amz-Expires", expires.ToString());
        CommonUtil.AddParam(httpRequest, "X-Amz-SignedHeaders", signedHeaders);

        HttpContext context = new HttpContext(sp, ObsConfig);
        string signature = CommonUtil.UrlEncode(V4Signer.GetTemporarySignature(httpRequest, context, iheaders, dateDict, signedHeaders, tempDict, signedHeadersList, null));

        CreateTemporarySignatureResponse response = new CreateTemporarySignatureResponse
        {
            SignUrl = ObsConfig.Endpoint.StartsWith("https", StringComparison.OrdinalIgnoreCase) ?
            "https://" : "http://",
        };

        response.SignUrl += host;

        if (ObsConfig.PathStyle && !string.IsNullOrEmpty(httpRequest.BucketName))
        {
            response.SignUrl += "/" + CommonUtil.UrlEncode(httpRequest.BucketName);
        }

        if (!string.IsNullOrEmpty(httpRequest.ObjectKey))
        {
            response.SignUrl += "/" + CommonUtil.UrlEncode(httpRequest.ObjectKey, null, "/");
        }

        bool isFirst = true;
        foreach (KeyValuePair<string, string> entry in httpRequest.Params)
        {
            if (isFirst)
            {
                response.SignUrl += "?";
                isFirst = false;
            }
            else
            {
                response.SignUrl += "&";
            }
            response.SignUrl += CommonUtil.UrlEncode(entry.Key);
            response.SignUrl += "=";
            response.SignUrl += CommonUtil.UrlEncode(entry.Value);
        }

        if (!string.IsNullOrEmpty(sp.Token))
        {
            response.SignUrl += "&" + iheaders.SecurityTokenHeader() + "=" + sp.Token;
        }
        response.SignUrl += "&X-Amz-Signature=" + signature;

        foreach (KeyValuePair<string, string> entry in httpRequest.Headers)
        {
            if (!entry.Key.Equals(Constants.CommonHeaders.Date))
            {
                response.ActualSignedRequestHeaders.Add(entry.Key, entry.Value);
            }
        }

        return response;
    }

    private CreateTemporarySignatureResponse CreateTemporarySignature(HttpRequest httpRequest, long expires, IHeaders iheaders)
    {
        DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        string expiresValue = (((DateTime.UtcNow.Ticks - dt1970.Ticks) / 10000000) + expires).ToString();
        httpRequest.Headers[Constants.CommonHeaders.Date] = expiresValue;

        HttpContext context = new HttpContext(sp, ObsConfig);

        IDictionary<string, string> SinerReturn = httpClient.GetSigner(new HttpContext(sp, ObsConfig)).GetSignature(httpRequest, context, iheaders);

        string signature = CommonUtil.UrlEncode(SinerReturn["Signature"]);

        CreateTemporarySignatureResponse response = new CreateTemporarySignatureResponse
        {
            SignUrl = ObsConfig.Endpoint.StartsWith("https", StringComparison.OrdinalIgnoreCase) ?
            "https://" : "http://",
        };

        response.SignUrl += httpRequest.GetHost(ObsConfig.Endpoint);

        if (ObsConfig.PathStyle && !string.IsNullOrEmpty(httpRequest.BucketName))
        {
            response.SignUrl += "/" + CommonUtil.UrlEncode(httpRequest.BucketName);
        }

        if (!string.IsNullOrEmpty(httpRequest.ObjectKey))
        {
            response.SignUrl += "/" + CommonUtil.UrlEncode(httpRequest.ObjectKey, null, "/");
        }

        string accessKeyIdPrefix = ObsConfig.AuthType == AuthTypeEnum.OBS ? "AccessKeyId=" : "AWSAccessKeyId=";
        response.SignUrl += "?" + accessKeyIdPrefix + sp.Ak + "&Expires=" + expiresValue;

        foreach (KeyValuePair<string, string> entry in httpRequest.Params)
        {
            response.SignUrl += "&";
            response.SignUrl += CommonUtil.UrlEncode(entry.Key);
            response.SignUrl += "=";
            response.SignUrl += CommonUtil.UrlEncode(entry.Value);
        }

        response.SignUrl += "&Signature=" + signature;

        foreach (KeyValuePair<string, string> entry in httpRequest.Headers)
        {
            if (!entry.Key.Equals(Constants.CommonHeaders.Date))
            {
                response.ActualSignedRequestHeaders.Add(entry.Key, entry.Value);
            }
        }

        return response;
    }

    /// <summary>
    /// Generate browser-based authentication parameters. Currently, this function is not supported.
    /// </summary>
    /// <param name="request">Request parameters</param>
    /// <returns>Response</returns>
    public CreatePostSignatureResponse CreatePostSignature(CreatePostSignatureRequest request)
    {
        throw new NotImplementedException();
    }
}
