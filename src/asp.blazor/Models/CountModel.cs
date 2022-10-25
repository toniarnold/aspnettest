namespace asp.blazor.Models
{
    /// <summary>
    /// For Async.razor
    /// </summary>
    public class CountModel
    {
        public CountModel(int initialize)
        {
            Counter = initialize;
        }

        public int Counter = 0;
    }
}