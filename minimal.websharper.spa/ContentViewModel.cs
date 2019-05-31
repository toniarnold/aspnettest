using asplib.Model;
using System.Collections.Generic;

namespace minimal.websharper.spa
{
    public class ContentViewModel : ViewModel<Content>
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