namespace asplib.Model
{
    /// <summary>
    /// Storage method for the persistence of M
    /// </summary>
    public enum Storage
    {
        /// <summary>
        /// ViewState (=DOM in WebSharper) is the least persistent storage, cleared when navigating to the URL
        /// </summary>
        ViewState,

        /// <summary>
        /// Session is the middle persistent storage, cleared when closing the browser
        /// </summary>
        Session,

        /// <summary>
        /// Database is the most persistent storage, cleared when persistent cookies are deleted
        /// </summary>
        Database,

        /// <summary>
        /// Header is only appropriate for formally stateless .NET Core API
        /// clients without any dependency on session or database storage
        /// </summary>
        Header,

        /// <summary>
        /// Browser JavaScript Window.sessionStorage via ProtectedSessionStorage
        /// </summary>
        SessionStorage,

        /// <summary>
        /// Browser JavaScript Window.localStorage via ProtectedLocalStorage
        /// </summary>
        LocalStorage,
    }
}