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
using OBS.Internal.Log;
using OBS.Internal.Negotiation;
using OBS.Model;
using OBSHttpClient = OBS.Internal.HttpClient;

namespace OBS;

#pragma warning disable CS0612 // 类型或成员已过时
#pragma warning disable SA1312 // Variable names should begin with lower-case letter

/// <summary>
/// Access an instance of ObsClient.
/// </summary>
public partial class ObsClient
{
    private OBSHttpClient httpClient = null!;
    private LocksHolder locksHolder = null!;
    private AuthTypeCache authTypeCache = null!;
    private volatile SecurityProvider sp = null!;

    internal delegate void DoValidateDelegate();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="accessKeyId">AK in the access key</param>
    /// <param name="secretAccessKey">SK in the access key</param>
    /// <param name="obsConfig">Configuration parameters of ObsClient</param>
    public ObsClient(string accessKeyId, string secretAccessKey, ObsConfig obsConfig) : this(accessKeyId, secretAccessKey, "", obsConfig)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="accessKeyId">AK in the access key</param>
    /// <param name="secretAccessKey">SK in the access key</param>
    /// <param name="securityToken">Security token</param>
    /// <param name="obsConfig">Configuration parameters of ObsClient</param>
    public ObsClient(string accessKeyId, string secretAccessKey, string securityToken, ObsConfig obsConfig)
    {
        Init(accessKeyId, secretAccessKey, securityToken, obsConfig);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="accessKeyId">AK in the access key</param>
    /// <param name="secretAccessKey">SK in the access key</param>
    /// <param name="endpoint">OBS endpoint</param>
    public ObsClient(string accessKeyId, string secretAccessKey, string endpoint) : this(accessKeyId, secretAccessKey, "", endpoint)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="accessKeyId">AK in the access key</param>
    /// <param name="secretAccessKey">SK in the access key</param>
    /// <param name="securityToken">Security token</param>
    /// <param name="endpoint">OBS endpoint</param>
    public ObsClient(string accessKeyId, string secretAccessKey, string securityToken, string endpoint)
    {
        ObsConfig obsConfig = new ObsConfig()
        {
            Endpoint = endpoint,
        };
        Init(accessKeyId, secretAccessKey, securityToken, obsConfig);
    }

    internal ObsConfig ObsConfig { get; set; } = null!;

    internal IParser GetIParser(HttpContext context)
    {
        return context.ChooseAuthType switch
        {
            AuthTypeEnum.V2 or AuthTypeEnum.V4 => V2Parser.GetInstance(httpClient.GetIHeaders(context)),
            _ => ObsParser.GetInstance(httpClient.GetIHeaders(context)),
        };
    }

    internal IConvertor GetIConvertor(HttpContext context)
    {
        return context.ChooseAuthType switch
        {
            AuthTypeEnum.V2 or AuthTypeEnum.V4 => V2Convertor.GetInstance(httpClient.GetIHeaders(context)),
            _ => ObsConvertor.GetInstance(httpClient.GetIHeaders(context)),
        };
    }

    internal void Init(string accessKeyId, string secretAccessKey, string securityToken, ObsConfig obsConfig)
    {
        if (obsConfig == null || string.IsNullOrEmpty(obsConfig.Endpoint))
        {
            throw new ObsException(Constants.InvalidEndpointMessage, ErrorType.Sender, Constants.InvalidEndpoint, "");
        }

        SecurityProvider sp = new SecurityProvider
        {
            Ak = accessKeyId,
            Sk = secretAccessKey,
            Token = securityToken,
        };

        this.sp = sp;
        ObsConfig = obsConfig;
        httpClient = new OBSHttpClient(ObsConfig);

        if (ObsConfig.PathStyle)
        {
            ObsConfig.AuthTypeNegotiation = false;
            if (ObsConfig.AuthType == AuthTypeEnum.OBS)
            {
                ObsConfig.AuthType = AuthTypeEnum.V2;
            }
        }

        if (ObsConfig.AuthTypeNegotiation)
        {
            locksHolder = new LocksHolder();
            authTypeCache = new AuthTypeCache();
        }

        //LoggerMgr.Initialize();

        if (LoggerMgr.IsWarnEnabled)
        {
            var sb = new StringBuilder();
            sb.Append("[OBS SDK Version=")
                .Append(Constants.ObsSdkVersion)
                .Append("];[")
                .Append("Endpoint=")
                .Append(obsConfig.Endpoint)
                 .Append("];[")
                 .Append("Access Mode=")
                 .Append(obsConfig.PathStyle ? "Path" : "Virtual Hosting")
                 .Append(']');
            LoggerMgr.Warn(sb.ToString());
        }
    }

    /// <summary>
    /// Refresh the temporary access key.
    /// </summary>
    /// <param name="accessKeyId">AK in the access key</param>
    /// <param name="secretAccessKey">SK in the access key</param>
    /// <param name="securityToken">Security token</param>
    public void Refresh(string accessKeyId, string secretAccessKey, string securityToken)
    {
        var sp = new SecurityProvider
        {
            Ak = accessKeyId,
            Sk = secretAccessKey,
            Token = securityToken,
        };
        this.sp = sp;
    }

    internal GetApiVersionResponse GetApiVersion(GetApiVersionRequest request)
    {
        return DoRequest<GetApiVersionRequest, GetApiVersionResponse>(request);
    }

    internal AuthTypeEnum? NegotiateAuthType(string? bucketName, bool async)
    {
        AuthTypeEnum? authType = AuthTypeEnum.V2;
        GetApiVersionRequest request = new GetApiVersionRequest
        {
            BucketName = bucketName,
        };
        try
        {
            GetApiVersionResponse response = async ? GetApiVersionAsync(request) : GetApiVersion(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if ((response.Headers.ContainsKey(Constants.ObsApiHeader)
                && response.Headers[Constants.ObsApiHeader].CompareTo("3.0") >= 0) || (response.Headers.ContainsKey(Constants.ObsApiHeaderWithPrefix)
                && response.Headers[Constants.ObsApiHeaderWithPrefix].CompareTo("3.0") >= 0))
                {
                    authType = AuthTypeEnum.OBS;
                }
            }
        }
        catch (ObsException ex)
        {
            int statusCode = Convert.ToInt32(ex.StatusCode);
            if (statusCode <= 0 || statusCode == 404 || statusCode >= 500)
            {
                throw;
            }

            if (LoggerMgr.IsInfoEnabled)
            {
                string msg = string.IsNullOrEmpty(bucketName) ? "The target server doesnot support OBS protocol, use S3 protocol" : string.Format("The target server doesnot support OBS protocol, use S3 protocol, Bucket:{0}", bucketName);
                LoggerMgr.Info(msg, ex);
            }
        }

        return authType;
    }

    private HttpContext BeforeRequest<T>(T request, DoValidateDelegate? doValidateDelegate, bool async) where T : ObsWebServiceRequest
    {
        if (request == null)
        {
            throw new ObsException(Constants.NullRequestMessage, ErrorType.Sender, Constants.NullRequest, "");
        }

        HttpContext context = new HttpContext(sp, ObsConfig);

        if (request is GetApiVersionRequest)
        {
            context.SkipAuth = true;
        }
        else
        {
            var _request = request as ObsBucketWebServiceRequest;
            if (_request != null && string.IsNullOrEmpty(_request.BucketName))
            {
                throw new ObsException(Constants.InvalidBucketNameMessage, ErrorType.Sender, Constants.InvalidBucketName, "");
            }
            if (ObsConfig.AuthTypeNegotiation)
            {
                AuthTypeEnum? authType;
                if (_request == null)
                {
                    //list buckets
                    authType = NegotiateAuthType(null, async);
                }
                else
                {
                    if ((authType = authTypeCache.GetAuthType(_request.BucketName)) == null)
                    {
                        lock (locksHolder.GetLock(_request.BucketName))
                        {
                            if ((authType = authTypeCache.GetAuthType(_request.BucketName)) == null)
                            {
                                if (request is CreateBucketRequest)
                                {
                                    authType = NegotiateAuthType(null, async);
                                }
                                else
                                {
                                    authType = NegotiateAuthType(_request.BucketName, async);
                                    authTypeCache.RefreshAuthType(_request.BucketName, authType!.Value);
                                    if (LoggerMgr.IsInfoEnabled)
                                    {
                                        LoggerMgr.Info(string.Format("Refresh auth type {0} for bucket {1}", authType, _request.BucketName));
                                    }
                                }
                            }
                        }
                    }
                }
                if (LoggerMgr.IsDebugEnabled)
                {
                    LoggerMgr.Debug(string.Format("Get auth type {0}", authType));
                }
                context.AuthType = authType;
            }
        }

        request.Sender = this;
        doValidateDelegate?.Invoke();

        if (LoggerMgr.IsInfoEnabled)
        {
            LoggerMgr.Info(request.GetAction() + " begin.");
        }
        return context;
    }

    private HttpRequest PrepareHttpRequest<T>(T request, HttpContext context) where T : ObsWebServiceRequest
    {
        IConvertor iconvertor = GetIConvertor(context);
        MethodInfo info = CommonUtil.GetTransMethodInfo(request.GetType(), iconvertor);
        HttpRequest? httpRequest = info.Invoke(iconvertor, [request]) as HttpRequest;
        if (httpRequest == null)
        {
            throw new ObsException(string.Format("Cannot trans request:{0} to HttpRequest", request.GetType()), ErrorType.Sender, "Trans error", "");
        }
        httpRequest.Endpoint = ObsConfig.Endpoint;
        httpRequest.PathStyle = ObsConfig.PathStyle;
        return httpRequest;
    }

    private K PrepareResponse<T, K>(T request, HttpContext context, HttpRequest httpRequest, HttpResponse httpResponse)
        where T : ObsWebServiceRequest
        where K : ObsWebServiceResponse
    {
        K? response;
        httpResponse.RequestUrl = httpRequest.GetUrlWithoutQuerys();
        IParser iparser = GetIParser(context);
        Type responseType = typeof(K);
        MethodInfo info = CommonUtil.GetParseMethodInfo(responseType, iparser);
        if (info != null)
        {
            response = info.Invoke(iparser, [httpResponse]) as K;
        }
        else
        {
            ConstructorInfo cinfo = responseType.GetConstructor([])!;
            response = cinfo.Invoke(null) as K;
        }
        if (response == null)
        {
            throw new ObsException(string.Format("Cannot parse HttpResponse to {0}", responseType), ErrorType.Sender, "Parse error", "");
        }
        CommonParser.ParseObsWebServiceResponse(httpResponse, response, httpClient.GetIHeaders(context));
        response.HandleObsWebServiceRequest(request);
        response.OriginalResponse = httpResponse;
        return response;
    }

    internal K DoRequest<T, K>(T request, DoValidateDelegate? doValidateDelegate)
        where T : ObsWebServiceRequest
        where K : ObsWebServiceResponse
    {
        HttpContext context = BeforeRequest(request, doValidateDelegate, false);
        DateTime reqTime = DateTime.Now;
        HttpRequest? httpRequest = null;
        try
        {
            httpRequest = PrepareHttpRequest(request, context);
            HttpResponse httpResponse = httpClient.PerformRequest(httpRequest, context);
            return PrepareResponse<T, K>(request, context, httpRequest, httpResponse);
        }
        catch (ObsException ex)
        {
            if ("CreateBucket".Equals(request.GetAction())
                && ex.StatusCode == HttpStatusCode.BadRequest
                && "Unsupported Authorization Type".Equals(ex.ErrorMessage)
                && ObsConfig.AuthTypeNegotiation
                && context.AuthType == AuthTypeEnum.OBS)
            {
                try
                {
                    if (httpRequest!.Content != null && httpRequest.Content.CanSeek)
                    {
                        httpRequest.Content.Seek(0, SeekOrigin.Begin);
                    }
                    context.AuthType = AuthTypeEnum.V2;
                    return PrepareResponse<T, K>(request, context, httpRequest, httpClient.PerformRequest(httpRequest, context));
                }
                catch (ObsException _ex)
                {
                    if (LoggerMgr.IsErrorEnabled)
                    {
                        LoggerMgr.Error(string.Format("{0} exception code: {1}, with message: {2}", request.GetAction(), _ex.ErrorCode, _ex.ErrorMessage));
                    }
                    throw;
                }
                catch (Exception _ex)
                {
                    if (LoggerMgr.IsErrorEnabled)
                    {
                        LoggerMgr.Error(string.Format("{0} exception with message: {1}", request.GetAction(), _ex.Message));
                    }
                    throw new ObsException(_ex.Message, _ex);
                }
            }

            if (LoggerMgr.IsErrorEnabled)
            {
                LoggerMgr.Error(string.Format("{0} exception code: {1}, with message: {2}", request.GetAction(), ex.ErrorCode, ex.ErrorMessage));
            }
            throw;
        }
        catch (Exception ex)
        {
            if (LoggerMgr.IsErrorEnabled)
            {
                LoggerMgr.Error(string.Format("{0} exception with message: {1}", request.GetAction(), ex.Message));
            }
            throw new ObsException(ex.Message, ex);
        }
        finally
        {
            if (request != null)
            {
                request.Sender = null!;
            }

            CommonUtil.CloseIDisposable(httpRequest);

            if (LoggerMgr.IsInfoEnabled)
            {
                LoggerMgr.Info(string.Format("{0} end, cost {1} ms", request?.GetAction(), (DateTime.Now - reqTime).TotalMilliseconds));
            }
        }
    }

    internal K DoRequest<T, K>(T request)
        where T : ObsWebServiceRequest
        where K : ObsWebServiceResponse
    {
        return DoRequest<T, K>(request, null);
    }
}
