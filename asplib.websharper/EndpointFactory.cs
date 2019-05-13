using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using WebSharper.Sitelets;
using asplib.Controllers;

namespace asplib.websharper
{
    public static class EndpointFactory
    {
        public static T Create<T>(Context<object> ctx)
            where T : new()
        {
            T endpoint;
            ctx.UserSession.LoginUser("toni", new TimeSpan(0, 30, 0));
            var u = ctx.UserSession.GetLoggedInUser();
            var context = (HttpContext)ctx.Environment["WebSharper.AspNetCore.HttpContext"];
            var sess = context.Session;
            var storageID = StorageControllerExtension.GetStorageID(typeof(T).Name);
            var sessionStorageID = StorageControllerExtension.GetSessionStorageID(typeof(T).Name);
            return new T();
        }
    }
}
