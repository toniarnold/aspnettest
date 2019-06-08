using asplib.Model;
using asplib.Remoting;
using System.Threading.Tasks;
using WebSharper;

namespace minimal.websharper.spa
{
    public static class StorageRemoting
    {
        // Static reference to the Model
        public static Content Content;

        /// <summary>
        /// Adds the specified content to the stored model and returns it to
        /// the caller.
        /// </summary>
        /// <param name="viewState">Optional ViewState string if the object has
        /// already been stored</param>
        /// <param name="item">Item string to add to the content</param>///
        /// <returns></returns>
        [Remote]
        public static Task<ContentViewModel> Add(string viewState, string item)
        {
            using (var content = StorageServer.Load<Content, ContentViewModel>(viewState, out Content))
            {
                content.Add(item);
                return content.ViewModelTask<Content, ContentViewModel>();
            }
        }

        /// <summary>
        /// Reloads the content after a storage mechanism change - the same
        /// as a NOP transition.
        /// </summary>
        /// <param name="viewState">State of the view.</param>
        /// <returns></returns>
        [Remote]
        public static Task<ContentViewModel> Reload(string viewState)
        {
            using (var content = StorageServer.Load<Content, ContentViewModel>(viewState, out Content))
            {
                return content.ViewModelTask<Content, ContentViewModel>();
            }
        }

        /// <summary>
        /// Saves the specified main. In case of ViewState, the Base64-encoded string
        /// of the object serialization is returned.
        /// Not yet used in this minimal project.
        /// </summary>
        /// <param name="stored">The stored content object.</param>
        /// <returns></returns>
        [Remote]
        public static Task<string> Save(ContentViewModel stored)
        {
            StorageServer.SaveDiscretely<Content>(stored);
            return Task.FromResult(stored.ViewState);
        }
    }
}