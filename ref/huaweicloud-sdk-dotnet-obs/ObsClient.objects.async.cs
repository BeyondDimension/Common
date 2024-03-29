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
using OBS.Model;

namespace OBS;

#pragma warning disable SA1130 // Use lambda syntax
#pragma warning disable SA1008 // Opening parenthesis should be spaced correctly
#pragma warning disable SA1410 // Remove delegate parenthesis when possible

public partial class ObsClient
{
    /// <summary>
    /// Start the asynchronous request for uploading an object.
    /// </summary>
    /// <param name="request">Parameters in an object upload request</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginPutObject(PutObjectRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for uploading an object.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns> Response to an object upload request</returns>
    public PutObjectResponse EndPutObject(IAsyncResult ar)
    {
        return EndDoRequest<PutObjectRequest, PutObjectResponse>(ar);
    }

    /// <summary>
    /// Start the asynchronous request for an appendable upload.
    /// </summary>
    /// <param name="request">Parameters in an appendable upload request</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginAppendObject(AppendObjectRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous appendable upload request.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to an appendable upload request</returns>
    public AppendObjectResponse EndAppendObject(IAsyncResult ar)
    {
        return EndDoRequest<AppendObjectRequest, AppendObjectResponse>(ar);
    }

    /// <summary>
    /// Start the asynchronous request for copying an object.
    /// </summary>
    /// <param name="request">Parameters in a request for copying an object</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginCopyObject(CopyObjectRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
            if (string.IsNullOrEmpty(request.SourceBucketName))
            {
                throw new ObsException(Constants.InvalidSourceBucketNameMessage, ErrorType.Sender, Constants.InvalidBucketName, "");
            }
            if (request.SourceObjectKey == null)
            {
                throw new ObsException(Constants.InvalidSourceObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for copying an object.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns> Response to an object copy request</returns>
    public CopyObjectResponse EndCopyObject(IAsyncResult ar)
    {
        return EndDoRequest<CopyObjectRequest, CopyObjectResponse>(ar);
    }

    /// <summary>
    /// Start the asynchronous request for uploading a part.
    /// </summary>
    /// <param name="request">Parameters in a request for uploading a part</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginUploadPart(UploadPartRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
            if (string.IsNullOrEmpty(request.UploadId))
            {
                throw new ObsException(Constants.InvalidUploadIdMessage, ErrorType.Sender, Constants.InvalidUploadId, "");
            }

            if (request.PartNumber <= 0)
            {
                throw new ObsException(Constants.InvalidPartNumberMessage, ErrorType.Sender, Constants.InvalidPartNumber, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for uploading a part.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to a part upload request</returns>
    public UploadPartResponse EndUploadPart(IAsyncResult ar)
    {
        UploadPartResponse response = EndDoRequest<UploadPartRequest, UploadPartResponse>(ar);
        HttpObsAsyncResult? result = ar as HttpObsAsyncResult;
        object[]? additionalState = result!.AdditionalState as object[];
        UploadPartRequest? request = additionalState![0] as UploadPartRequest;
        response.PartNumber = request!.PartNumber;
        return response;
    }

    /// <summary>
    /// Start the asynchronous request for copying a part.
    /// </summary>
    /// <param name="request">Parameters in a request for copying a part</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginCopyPart(CopyPartRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
            if (string.IsNullOrEmpty(request.UploadId))
            {
                throw new ObsException(Constants.InvalidUploadIdMessage, ErrorType.Sender, Constants.InvalidUploadId, "");
            }

            if (request.PartNumber <= 0)
            {
                throw new ObsException(Constants.InvalidPartNumberMessage, ErrorType.Sender, Constants.InvalidPartNumber, "");
            }

            if (string.IsNullOrEmpty(request.SourceBucketName))
            {
                throw new ObsException(Constants.InvalidSourceBucketNameMessage, ErrorType.Sender, Constants.InvalidBucketName, "");
            }
            if (request.SourceObjectKey == null)
            {
                throw new ObsException(Constants.InvalidSourceObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for copying a part.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns> Response to a part copy request</returns>
    public CopyPartResponse EndCopyPart(IAsyncResult ar)
    {
        CopyPartResponse response = EndDoRequest<CopyPartRequest, CopyPartResponse>(ar);
        HttpObsAsyncResult? result = ar as HttpObsAsyncResult;
        object[]? additionalState = result!.AdditionalState as object[];
        CopyPartRequest? request = additionalState![0] as CopyPartRequest;
        response.PartNumber = request!.PartNumber;
        return response;
    }

    /// <summary>
    /// End the asynchronous request for downloading an object.
    /// </summary>
    /// <param name="request">Parameters in an object download request</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginGetObject(GetObjectRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for downloading an object.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to an object download request</returns>
    public GetObjectResponse EndGetObject(IAsyncResult ar)
    {
        GetObjectResponse response = EndDoRequest<GetObjectRequest, GetObjectResponse>(ar);
        HttpObsAsyncResult? result = ar as HttpObsAsyncResult;
        object[]? additionalState = result!.AdditionalState as object[];
        GetObjectRequest? request = additionalState![0] as GetObjectRequest;
        response.BucketName = request!.BucketName!;
        response.ObjectKey = request.ObjectKey;
        return response;
    }

    /// <summary>
    /// Start the asynchronous request for obtaining object properties.
    /// </summary>
    /// <param name="request">Parameters in a request for obtaining object properties</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginGetObjectMetadata(GetObjectMetadataRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for obtaining object properties.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to a request for obtaining object properties</returns>
    public GetObjectMetadataResponse EndGetObjectMetadata(IAsyncResult ar)
    {
        GetObjectMetadataResponse response = EndDoRequest<GetObjectMetadataRequest, GetObjectMetadataResponse>(ar);
        HttpObsAsyncResult? result = ar as HttpObsAsyncResult;
        object[]? additionalState = result!.AdditionalState as object[];
        GetObjectMetadataRequest? request = additionalState![0] as GetObjectMetadataRequest;
        response.BucketName = request!.BucketName!;
        response.ObjectKey = request.ObjectKey;
        return response;
    }

    /// <summary>
    /// Start the asynchronous request for initializing a multipart upload.
    /// </summary>
    /// <param name="request">Parameters in a request for initializing a multipart upload</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginInitiateMultipartUpload(InitiateMultipartUploadRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for initializing a multipart upload.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to a request for initializing a multipart upload</returns>
    public InitiateMultipartUploadResponse EndInitiateMultipartUpload(IAsyncResult ar)
    {
        return EndDoRequest<InitiateMultipartUploadRequest, InitiateMultipartUploadResponse>(ar);
    }

    /// <summary>
    /// Start the asynchronous request for combining parts.
    /// </summary>
    /// <param name="request">Parameters in a request for combining parts</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginCompleteMultipartUpload(CompleteMultipartUploadRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
            if (string.IsNullOrEmpty(request.UploadId))
            {
                throw new ObsException(Constants.InvalidUploadIdMessage, ErrorType.Sender, Constants.InvalidUploadId, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for combining parts.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to the request for combining parts</returns>
    public CompleteMultipartUploadResponse EndCompleteMultipartUpload(IAsyncResult ar)
    {
        return EndDoRequest<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse>(ar);
    }

    /// <summary>
    /// Start the asynchronous request for aborting a multipart upload.
    /// </summary>
    /// <param name="request">Parameters in a request for aborting a multipart upload</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginAbortMultipartUpload(AbortMultipartUploadRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
            if (string.IsNullOrEmpty(request.UploadId))
            {
                throw new ObsException(Constants.InvalidUploadIdMessage, ErrorType.Sender, Constants.InvalidUploadId, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for aborting a multipart upload.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to the request for aborting a multipart upload</returns>
    public AbortMultipartUploadResponse EndAbortMultipartUpload(IAsyncResult ar)
    {
        return EndDoRequest<AbortMultipartUploadRequest, AbortMultipartUploadResponse>(ar);
    }

    /// <summary>
    /// Start the asynchronous request for listing uploaded parts.
    /// </summary>
    /// <param name="request">Parameters in a request for listing uploaded parts</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginListParts(ListPartsRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
            if (string.IsNullOrEmpty(request.UploadId))
            {
                throw new ObsException(Constants.InvalidUploadIdMessage, ErrorType.Sender, Constants.InvalidUploadId, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for listing uploaded parts.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to a request for listing uploaded parts</returns>
    public ListPartsResponse EndListParts(IAsyncResult ar)
    {
        return EndDoRequest<ListPartsRequest, ListPartsResponse>(ar);
    }

    /// <summary>
    /// Start the asynchronous request for deleting an object.
    /// </summary>
    /// <param name="request">Parameters in an object deletion request</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginDeleteObject(DeleteObjectRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous object deletion request.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to the object deletion request</returns>
    public DeleteObjectResponse EndDeleteObject(IAsyncResult ar)
    {
        return EndDoRequest<DeleteObjectRequest, DeleteObjectResponse>(ar);
    }

    /// <summary>
    /// Start the asynchronous request for deleting objects in a batch.
    /// </summary>
    /// <param name="request">Parameters in a request for deleting objects in a batch</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginDeleteObjects(DeleteObjectsRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for deleting objects in a batch.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to an object batch deletion request</returns>
    public DeleteObjectsResponse EndDeleteObjects(IAsyncResult ar)
    {
        return EndDoRequest<DeleteObjectsRequest, DeleteObjectsResponse>(ar);
    }

    /// <summary>
    /// Start the asynchronous request for restoring an Archive object.
    /// </summary>
    /// <param name="request">Parameters in a request for restoring an Archive object</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginRestoreObject(RestoreObjectRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for restoring an Archive object.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to a request for restoring an Archive object</returns>
    public RestoreObjectResponse EndRestoreObject(IAsyncResult ar)
    {
        return EndDoRequest<RestoreObjectRequest, RestoreObjectResponse>(ar);
    }

    /// <summary>
    /// Start the asynchronous request for obtaining an object ACL.
    /// </summary>
    /// <param name="request">Parameters in a request for obtaining an object ACL</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginGetObjectAcl(GetObjectAclRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for obtaining an object ACL.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to a request for obtaining an object ACL</returns>
    public GetObjectAclResponse EndGetObjectAcl(IAsyncResult ar)
    {
        return EndDoRequest<GetObjectAclRequest, GetObjectAclResponse>(ar);
    }

    /// <summary>
    /// Start the asynchronous request for configuring an object ACL.
    /// </summary>
    /// <param name="request">Parameters in a request for configuring an object ACL</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginSetObjectAcl(SetObjectAclRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, delegate ()
        {
            if (request.ObjectKey == null)
            {
                throw new ObsException(Constants.InvalidObjectKeyMessage, ErrorType.Sender, Constants.InvalidObjectKey, "");
            }
        }, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for configuring an object ACL.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to a request for configuring an object ACL</returns>
    public SetObjectAclResponse EndSetObjectAcl(IAsyncResult ar)
    {
        return EndDoRequest<SetObjectAclRequest, SetObjectAclResponse>(ar);
    }

    /// <summary>
    /// Start the asynchronous request for querying whether a object exists.
    /// </summary>
    /// <param name="request">Parameters in a request for querying whether a object exists</param>
    /// <param name="callback">Asynchronous request callback function</param>
    /// <param name="state">Asynchronous request status object</param>
    /// <returns>Response to the asynchronous request</returns>
    public IAsyncResult BeginHeadObject(HeadObjectRequest request, AsyncCallback callback, object state)
    {
        return BeginDoRequest(request, callback, state);
    }

    /// <summary>
    /// End the asynchronous request for querying whether a object exists.
    /// </summary>
    /// <param name="ar">Response to the asynchronous request</param>
    /// <returns>Response to a request for querying whether a object exists</returns>
    public bool EndHeadObject(IAsyncResult ar)
    {
        try
        {
            EndDoRequest<HeadObjectRequest, ObsWebServiceResponse>(ar);
            return true;
        }
        catch (ObsException e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            throw;
        }
    }
}


