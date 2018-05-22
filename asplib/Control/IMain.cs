/*
 * Interface for the SMC AppClass
 */

using statemap;

namespace asplib.Control
{
    /// <summary>
    /// Interface for the SMC AppClass
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="S"></typeparam>
    public interface IMain<F, S>
        where F : statemap.FSMContext
        where S : statemap.State
    {
        S State { get; set; }
        F Fsm { get; }
    }
}
