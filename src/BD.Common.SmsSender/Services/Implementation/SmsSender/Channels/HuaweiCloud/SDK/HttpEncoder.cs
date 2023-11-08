//------------------------------------------------------------------------------
// based on https://github.com/Microsoft/referencesource/blob/master/System.Web/Util/HttpEncoder.cs
//   and https://github.com/Microsoft/referencesource/blob/master/System.Web/Util/HttpEncoderUtility.cs
//
// <copyright file="HttpEncoder.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <copyright file="HttpEncoderUtility.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

/*
 * Copyright (c) 2009 Microsoft Corporation
 */

namespace APIGATEWAY_SDK
{
    public partial class Signer
    {

        private static string UrlEncode(string value)
        {
            return HttpUtility.UrlEncode(value);
        }

    }
}