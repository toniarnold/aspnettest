using asplib.Model;
using FractalSharp.Algorithms.Fractals;
using statemap;
using System;
using System.IO;
using System.Threading.Tasks;
using static ImageContext.RenderMap;

namespace mandelbrot.image
{
    /// <summary>
    /// Image processor modeled after FractalSharp.ExampleApp
    /// All state transition methods are synchronized.
    /// </summary>
    [Serializable]
    [Clsid("1b2c5bf8-214a-4d8c-a9f2-5cf01e7d1c05")]
    public class Image : ISmcTask<Image, ImageContext, ImageContext.ImageState>

    {
        #region Fsm

        public Image()
        {
            this.Fsm = new ImageContext(this);
            AddInternalStateChangedHandler();
            ReachedState = Fsm.State;
            Guid = Guid.NewGuid();
        }

        #region IStored<M>

        [NonSerialized] // not possible on auto property
        private ViewModel<Image> viewModel;

        public ViewModel<Image> ViewModel
        {
            get => this.viewModel;
            set => this.viewModel = value;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Save();
                }
                disposedValue = true;
            }
        }

        #endregion IStored<M>

        #region ISmcTask

        public ImageContext Fsm { get; private set; }

        public ImageContext.ImageState State =>
            this.GetState<Image, ImageContext, ImageContext.ImageState>();

        #endregion ISmcTask

        #endregion Fsm

        // ---------- Constructor members:

        /// <summary>
        /// EscapeTimeParams<double>-typed accessor for the parameters transparently
        /// converting/persisting to the serializable Params representation
        /// </summary>
        internal EscapeTimeParams<double> EscapeTimeParams
        {
            set => Params = ParamsConvert.FromEscapeTimeParams(value);
            get => ParamsConvert.ToEscapeTimeParams(Params);
        }

        // ---------- Transient (nonserialized) members:

        [NonSerialized]
        private byte[] _image;

        // Synchronized Lazy Load for the empty image placeholder:
        [NonSerialized]
        private static byte[] _emptyImage;

        [NonSerialized]
        private static readonly object lockEmptyImage = new object();

        private static byte[] EmptyImage
        {
            get
            {
                lock (lockEmptyImage)
                {
                    if (_emptyImage == null)
                    {
                        var dllDir = AppDomain.CurrentDomain.BaseDirectory;
                        var image = Path.Join(dllDir, "empty.png");
                        _emptyImage = File.ReadAllBytes(image);
                    }
                }
                return _emptyImage;
            }
        }

        [NonSerialized]
        internal Renderer Renderer;

        // ---------- Public accessors:

        /// <summary>
        /// Image coordinates in the front end
        /// </summary>
        public Coordinates Coordinates { get; set; }

        /// <summary>
        /// Image resolution in the browser
        /// </summary>
        public Resolution Resolution { get; set; }

        /// <summary>
        /// Sufficient Image specification with .Key as key for the image cache
        /// </summary>
        public Specification Specification { get; private set; }

        /// <summary>
        /// Serializable EscapeTimeParams<double> representation for the FractalSharp parameters
        /// </summary>
        public Params Params { get; internal set; }

        /// <summary>
        /// img src identifier
        /// </summary>
#pragma warning disable CA2235 // Mark all non-serializable fields
        public Guid Guid { get; private set; }
#pragma warning restore CA2235 // Mark all non-serializable fields

        /// <summary>
        /// Querying the Fsm in the middle of a transition throws,
        /// thus provide a safe accessor for the last reached state
        /// </summary>
        public ImageContext.ImageState ReachedState
        {
            get => this.Fsm.valueOf(_reachedStateId);
            private set => _reachedStateId = value.Id;
        }

        private int _reachedStateId; // int, as ImageContext.ImageState is not serializable

#pragma warning disable CA1819 // Properties should not return arrays

        /// <summary>
        /// The rendered image or empty.png if it is not yet ready
        /// </summary>
        public byte[] Bytes
        {
            get => IsReady ? _image : EmptyImage;
            set => _image = value; // for setting by the client receiving the image via separate HTTP request
        }

#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// True when rendering is finished
        /// </summary>
        public bool IsReady => ReachedState == Ready;

        /// <summary>
        /// True when the image is in a newly initialized state
        /// </summary>
        public bool IsEmpty => ReachedState == Empty;

        /// <summary>
        /// Public constructor for initializing a new image
        /// </summary>
        /// <param name="coordinates">coordinates in the front end  grid</param>
        public Image(Coordinates coordinates) : this()
        {
            Coordinates = coordinates;
        }

        /// <summary>
        /// Public constructor for instantiating a new image in mandelbrot.renderer
        /// (Without coordinates) directly into the intermediate state Parameters
        /// </summary>
        /// <param name="coordinates">coordinates in the front end  grid</param>
        /// <param name="resolution">single image tile resolution</param>
        public Image(Params parameters, Resolution resolution) : this()
        {
            Params = parameters;     // overwrite computed 0 Params
            Resolution = resolution;
            Specification = new Specification(parameters, resolution);
            Fsm.State = Parameters;
        }

        /// <summary>
        /// Called after each state change
        /// </summary>
        /// <param name="handler">Handler(object sender, StateChangeEventArgs args)</param>
        public void AddStateChangedHandler(StateChangeEventHandler handler)
        {
            Fsm.StateChange += handler;
        }

        /// <summary>
        /// Assigned event handlers are not serialized - therefore this public
        /// hook to recreate the internal event handler when needed.
        /// </summary>
        public void AddInternalStateChangedHandler()
        {
            AddStateChangedHandler(StateChanged);
        }

        private void StateChanged(object sender, StateChangeEventArgs args)
        {
            ReachedState = (ImageContext.ImageState)args.NewState();
        }

        /// <summary>
        /// Compute the fractal parameters out of the image coordinates when in Empty state
        /// </summary>
        public void ComputeParams(Resolution resolution)
        {
            var zoom = Coordinates.Z >= 0 ? 2.0 * (double)Coordinates.Z + 1 :
                                            1 / -(2.0 * (double)Coordinates.Z - 1);
            // WRONG ASSUMPTIONS:
            // zoom 1 denotes a Zero grid -1 .. +1 with 1 tile of size 2
            // zoom 3 denotes a Zero grid -1 .. +1 with 3 tiles of size 2/3
            // zoom 5 denotes a Zero grid -1 .. +1 with 5 tiles of size 2/5
            // zoom 0.3333 denotes a grid with tiles of size 6 (2*3)
            // zoom 0.2 denotes a grid width tiles of size 10 (2*5)
            // (real, imag) denote the center position of the tile

            // EMPIRICALLY CORRECTED:
            var tilesize = 4.0 / zoom;
            var real = (double)Coordinates.Y * tilesize * (double)resolution.Width / (double)resolution.Height;
            var imag = -(double)Coordinates.X * tilesize;
            Params = new Params(zoom, new Complex(real, imag), 4.0); // EscapeRadius constant
            Specification = new Specification(Params, resolution);
            Resolution = resolution;
        }

        /// <summary>
        /// Asynchronously render the image atomically
        /// </summary>
        public Task RenderAsync()
        {
            return Task.Run(() => Render());
        }

        /// <summary>
        /// Render the image atomically
        /// </summary>
        public void Render()
        {
            Renderer = new Renderer(EscapeTimeParams, Resolution.Width, Resolution.Height);
            while (Fsm.State != Ready)
            {
                Fsm.Process(); // atomic calls to methods on Renderer
                if (Fsm.State == Rendered)
                    _image = Renderer.Image;
            }
            Renderer = null; // immediately free rendering buffers
        }

        /// <summary>
        /// Size estimate for ICacheEntry.Size
        /// </summary>
        /// <returns></returns>
        public long? Size()
        {
            return Serialization.Serialize(this).Length +
                    this.Bytes.Length; // NonSerialized
        }
    }
}