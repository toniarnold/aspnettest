namespace asplib.Model
{
    /// <summary>
    /// Interface for an SMC "Task class" which contains the "Task FSM"
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="S"></typeparam>
    public interface ISmcTask<M, F, S> : IStored<M>
        where M : class, IStored<M>, new()
        where F : statemap.FSMContext
        where S : statemap.State
    {
        F Fsm { get; }
        S State { get; }
    }

    /// <summary>
    /// Implementation of GetState/SetState only to avoid requiring inheritance
    /// </summary>
    public static class SmcTaskExtension
    {
        public static S GetState<M, F, S>(this ISmcTask<M, F, S> inst)
            where M : class, IStored<M>, new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            return (S)inst.Fsm.GetType().GetProperty("State").GetValue(inst.Fsm);
        }

        public static void SetState<M, F, S>(this ISmcTask<M, F, S> inst, S value)
            where M : class, IStored<M>, new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            inst.Fsm.GetType().GetProperty("State").SetValue(inst, value);
        }

        /// <summary>
        /// Must explicitly be called after deserialization.
        /// </summary>
        public static void SetOwner<M, F, S>(this ISmcTask<M, F, S> inst)
            where M : class, IStored<M>, new()
            where F : statemap.FSMContext
            where S : statemap.State
        {
            inst.Fsm.GetType().GetProperty("Owner").SetValue(inst.Fsm, inst);
        }
    }
}