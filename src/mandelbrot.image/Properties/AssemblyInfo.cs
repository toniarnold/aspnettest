using System.Runtime.CompilerServices;

// Make State in ImageContext.RenderMap visible to all clients
[assembly: InternalsVisibleTo("mandelbrot.imageviewmodel.fs")]
[assembly: InternalsVisibleTo("mandelbrot.renderer")]
[assembly: InternalsVisibleTo("mandelbrot.frontend")]
[assembly: InternalsVisibleTo("mandelbrot.image.test")]
[assembly: InternalsVisibleTo("mandelbrot.renderer.test")]
[assembly: InternalsVisibleTo("mandelbrot.frontend.test")]