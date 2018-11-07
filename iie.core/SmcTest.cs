using asplib.Controllers;
using NUnit.Framework;

namespace iie
{
    /// <summary>
    /// Base class for IE tests with accessors for ISmcControl
    /// </summary>
    [TestFixture]
    public abstract class SmcTest<C, F, S> : IETest<C>
        where C : SmcController<F, S>
        where F : statemap.FSMContext
        where S : statemap.State
    {
        protected F Fsm
        {
            get { return this.Controller.Fsm; }
        }

        protected S State
        {
            get { return this.Controller.State; }
        }
    }
}
