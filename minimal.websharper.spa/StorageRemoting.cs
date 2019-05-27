using asplib;
using asplib.Model;
using iie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using WebSharper;

namespace minimal.websharper.spa
{
    public static class StorageRemoting
    {
        // Static reference to the Model
        public static Content refContent;

        /// <summary>
        /// Adds the specified content to the stored model and returns it to
        /// the caller.
        /// </summary>
        /// <param name="viewState">Optional ViewState string if the object has
        /// already been stored</param>
        /// <param name="item">Item string to add to the content</param>/// 
        /// <returns></returns>
        [Remote]
        public static Task<StoredContent> Add(string viewState, string item)
        {
            var stored = StorageServer.Load<StoredContent, Content>(viewState, ref refContent);
            stored.Main.Add(item);
            // Immediately safe after the state transition and make the new
            // state visible to WebSharper in the ViewState.
            stored.ViewState = StorageServer.Save<Content>(stored);
            return Task.FromResult(stored);
        }


        /// <summary>
        /// Reloads the content after a storage mechanism change - the same
        /// as a NOP transition.
        /// </summary>
        /// <param name="viewState">State of the view.</param>
        /// <returns></returns>
        [Remote]
        public static Task<StoredContent> Reload(string viewState)
        {
            var stored = StorageServer.Load<StoredContent, Content>(viewState, ref refContent);
            stored.ViewState = StorageServer.Save<Content>(stored);
            return Task.FromResult(stored);
        }

        /// <summary>
        /// Saves the specified main. In case of ViewState, the Base64-encoded string
        /// of the object serialization is returned.
        /// Not yet used in this minimal project.
        /// </summary>
        /// <param name="stored">The stored content object.</param>
        /// <returns></returns>
        [Remote]
        public static Task<string> Save(StoredContent stored)
        {
            StorageServer.SaveDiscretely<Content>(stored);
            return Task.FromResult(stored.ViewState);
        }
    }
}
