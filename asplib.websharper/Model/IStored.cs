using asplib.Remoting;
using System;
using System.Threading.Tasks;

namespace asplib.Model
{
    /// <summary>
    /// Interface for the stored Main class M. IDisposable.Dispose() is
    /// somewhat misused as Save() to allow implementing [Remote] methods as
    /// terse as possible with the using statement. As an extension method
    /// cannot implement IDisposable.Dispose(), the model class must implement
    /// it itself - and it additionally needs to be marked as sealed.
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <seealso cref="System.IDisposable" />
    public interface IStored<M> : IDisposable
        where M : class, IStored<M>, new()
    {
        ViewModel<M> ViewModel { get; set; }
    }

    public static class StoredExtension
    {
        /// <summary>
        /// The Dispose() extension method is called implicitly before the
        /// ViewModel is returned and saves the M object either to the returned
        /// ViewState or implicitly on the server.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="inst">The inst.</param>
        public static void DisposeSave<M>(this IStored<M> inst)
            where M : class, IStored<M>, new()
        {
            inst.ViewModel.ViewState = StorageServer.Save<M>(inst.ViewModel);
        }

        public static Task<V> ViewModelTask<M, V>(this IStored<M> inst)
            where M : class, IStored<M>, new()
            where V : ViewModel<M>, new()
        {
            return Task.FromResult((V)inst.ViewModel);
        }
    }
}