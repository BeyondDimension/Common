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

namespace OBS.Model;

public abstract class AbstractAccessControlList
{
    private IList<Grant>? grants;

    /// <summary>
    /// User or user group ACL
    /// </summary>
    /// <remarks>
    /// <para>
    /// Optional parameter
    /// </para>
    /// </remarks>
    public IList<Grant> Grants
    {
        get => grants ??= new List<Grant>();
        set => grants = value;
    }

    /// <summary>
    /// Grant permissions to a user or user group (grantee).
    /// </summary>
    /// <param name="grantee"> Grantee name </param>
    /// <param name="permission">Permission information</param>
    public void AddGrant(Grantee grantee, PermissionEnum permission)
    {
        Grant grant = new Grant
        {
            Grantee = grantee,
            Permission = permission,
        };
        Grants.Add(grant);
    }

    /// <summary>
    /// Remove permissions from a grantee.
    /// </summary>
    /// <param name="grantee"> Grantee name </param>
    /// <param name="permission">Permission information</param>
    public void RemoveGrant(Grantee grantee, PermissionEnum permission)
    {
        foreach (Grant grant in Grants)
        {
            if (grant.Grantee.Equals(grantee) && grant.Permission == permission)
            {
                Grants.Remove(grant);
                break;
            }
        }
    }

    /// <summary>
    /// Remove all permissions from a grantee.
    /// </summary>
    /// <param name="grantee"> Grantee name </param>
    public void RemoveGrant(Grantee grantee)
    {
        IList<Grant> removeList = new List<Grant>();
        foreach (Grant grant in Grants)
        {
            if (grant.Grantee.Equals(grantee))
            {
                removeList.Add(grant);
            }
        }
        foreach (Grant grant in removeList)
        {
            Grants.Remove(grant);
        }
    }
}


