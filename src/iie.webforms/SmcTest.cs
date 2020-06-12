using asplib.View;
using NUnit.Framework;

namespace iie
{
    /// <summary>
    /// Base class for IE tests with accessors for ISmcControl
    /// </summary>
    [TestFixture]
    public abstract class SmcTest<M, F, S> : StorageTest<M>
        where M : new()
        where F : statemap.FSMContext
        where S : statemap.State
    {
        protected F Fsm
        {
            get { return this.MainControl.Fsm; }
        }

        protected S State
        {
            get { return this.MainControl.State; }
        }

        protected new ISmcControl<M, F, S> MainControl
        {
            get { return (ISmcControl<M, F, S>)ControlRootExtension.GetRoot(); }
        }
    }
}