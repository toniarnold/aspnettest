using asplib.Common;
using asplib.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Specialized;

namespace asplib.Controllers
{
    /// <summary>
    /// Extension interface for a Controller to make it persistent across requests.
    /// </summary>
    public interface IStorage : IStaticController
    {

    }

    /// <summary>
    /// Extension implementation with storage dependency
    /// </summary>
    public static class StorageController
    {

    }
}
