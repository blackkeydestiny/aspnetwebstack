// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Web.Http.Routing
{
    public interface IHttpVirtualPathData
    {
        /*
         * HttpRoute对象
         * **/
        IHttpRoute Route { get; }

        /*
         * 根据路由模板和指定路由变量生成一个完整的URL
         * **/
        string VirtualPath { get; set; }
    }
}
