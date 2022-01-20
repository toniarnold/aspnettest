namespace asplib.Services
{
    /// <summary>
    /// Global static accessor for the current T Main object for white box unit tests
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class MainAccessor<T>
    {
        public static T? Instance { get; set; }
    }
}