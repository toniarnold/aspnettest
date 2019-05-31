using asplib.Model;
using System;
using System.Collections.Generic;

namespace minimal.websharper.spa
{
    /// <summary>
    /// List<string> wrapper with a Clsid for database storage
    /// as a minimal serialized main Model class.
    /// </summary>
    [Serializable]
    [Clsid("F532E3FB-057C-4849-99B6-F288C20B788C")]
    public sealed class Content : List<string>, IStored<Content>
    {
        [NonSerialized] // not possible on auto property
        private ViewModel<Content> viewModel;

        public ViewModel<Content> ViewModel
        {
            get { return this.viewModel; }
            set { this.viewModel = value; }
        }

        public Content() : base()
        {
        }

        public Content(IEnumerable<string> content) : base(content)
        {
        }

        public void Dispose()
        {
            this.DisposeSave();
        }
    }
}