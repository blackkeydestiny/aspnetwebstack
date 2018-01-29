// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace System.Web.Http.Routing
{
    public interface IHttpRouteData
    {
        /*
         * HttpRoute对象
         * **/
        IHttpRoute Route { get; }

        /*
         * 解析出来的路由变量
         * **/
        IDictionary<string, object> Values { get; }
    }
}
