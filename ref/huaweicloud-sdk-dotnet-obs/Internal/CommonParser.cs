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
using System.Xml;

namespace OBS.Internal
{
    internal static class CommonParser
    {

        public static void ParseObsWebServiceResponse(HttpResponse httpResponse, ObsWebServiceResponse response, IHeaders iheaders)
        {
            response.StatusCode = httpResponse.StatusCode;
            if (httpResponse.Headers.ContainsKey(iheaders.RequestIdHeader()))
            {
                response.RequestId = httpResponse.Headers[iheaders.RequestIdHeader()];
            }
            if (httpResponse.Headers.TryGetValue(Constants.CommonHeaders.ContentLength, out string? value))
            {
                response.ContentLength = Convert.ToInt64(value);
            }

            foreach (KeyValuePair<string, string> header in httpResponse.Headers)
            {
                string key = header.Key;
                if (key.StartsWith(iheaders.HeaderMetaPrefix()))
                {
                    key = key[iheaders.HeaderMetaPrefix().Length..];
                }
                else if (key.StartsWith(iheaders.HeaderPrefix()))
                {
                    key = key[iheaders.HeaderPrefix().Length..];
                }
                else if (key.StartsWith(Constants.ObsHeaderMetaPrefix))
                {
                    key = key[Constants.ObsHeaderMetaPrefix.Length..];
                }
                else if (key.StartsWith(Constants.ObsHeaderPrefix))
                {
                    key = key[Constants.ObsHeaderPrefix.Length..];
                }
                response.Headers.Add(key, header.Value);
            }
        }

        public static void ParseErrorResponse(Stream stream, ObsException exception)
        {
            using XmlReader reader = XmlReader.Create(stream);
            while (reader.Read())
            {
                if ("Code".Equals(reader.Name))
                {
                    exception.ErrorCode = reader.ReadString();
                }
                else if ("Message".Equals(reader.Name))
                {
                    exception.ErrorMessage = reader.ReadString();
                }
                else if ("RequestId".Equals(reader.Name))
                {
                    exception.RequestId = reader.ReadString();
                }
                else if ("HostId".Equals(reader.Name))
                {
                    exception.HostId = reader.ReadString();
                }
            }
        }
    }
}
