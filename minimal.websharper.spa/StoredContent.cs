using asplib.Model;
using System;
using System.Collections.Generic;

namespace minimal.websharper.spa
{
    /// <summary>
    /// List<string> wrapper with a Clsid for database storage
    /// </summary>
    [Serializable]
    [Clsid("F532E3FB-057C-4849-99B6-F288C20B788C")]
    public class Content : List<string>
    {
        public Content() : base()
        {
        }

        public Content(IEnumerable<string> content) : base(content)
        {
        }
    }

    /// <summary>
    /// Stored<Content> with a serialized Items collection.
    /// </summary>
    /// <seealso cref="asplib.Model.Stored{minimal.websharper.spa.Content}" />
    public class StoredContent : Stored<Content>
    {
        /// <summary>
        /// Expose the Content collection as a list type recognized by WebSharper.
        /// </summary>
        public List<string> Content;

        public override void LoadMembers()
        {
            this.Content = new List<string>(this.Main);
        }

        public override void SaveMembers()
        {
            this.Main = new Content(this.Content);
        }
    }
}