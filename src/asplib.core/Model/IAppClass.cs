namespace asplib.Model
{
    /// <summary>
    /// Interface for the SMC AppClass
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="S"></typeparam>
    public interface IAppClass<F, S>
        where F : statemap.FSMContext
        where S : statemap.State
    {
        S State { get; }
        F Fsm { get; }
    }

    public static class IMainExtension
    {
        /// <summary>
        /// Must explicitly be called after deserialization (as the AppClass itself is not serializable in the FSM context).
        /// </summary>
        public static void SetOwner<F, S>(this IAppClass<F, S> main)
            where F : statemap.FSMContext
            where S : statemap.State
        {
            main.Fsm.GetType().GetProperty("Owner")!.SetValue(main.Fsm, main);  // Owner is generated thus guaranteed
        }

        /// <summary>
        /// The State property is only generated in the _sm.cs state machime
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="main"></param>
        public static S GetState<F, S>(this IAppClass<F, S> main)
            where F : statemap.FSMContext
            where S : statemap.State
        {
            return (S)main.Fsm.GetType().GetProperty("State")!.GetValue(main.Fsm)!;
        }
    }
}