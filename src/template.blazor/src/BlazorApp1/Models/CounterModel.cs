using asplib.Model;

namespace BlazorApp1.Models
{
    /// <summary>
    /// The persisted model object with the counter
    /// </summary>
    [Serializable]
    [Clsid("2a3a45d9-2d59-47e1-b746-130e3d614d77")]
    public class CounterModel
    {
        public int CurrentCount { get; private set; } = 0;

        public void IncrementCount()
        {
            CurrentCount++;
        }
    }
}