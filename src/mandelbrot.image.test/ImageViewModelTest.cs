using NUnit.Framework;

namespace mandelbrot.image.test
{
    public class ImageViewModelTest
    {
        [Test]
        public void ImageViewModelLifecycleTest()
        {
            // At the very first, the client requests a new image for a specific grid tile
            // It contains no serialized Fsm yet, is therefore a "flat" ViewModel
            var coord = new Coordinates(0, 0, 1);
            var clientImage = new ImageViewModel(coord, new Resolution(16, 16));
            var clientImageMsg = clientImage.ToArraySegment(); // in the test server side serialization

            // --> Send the image with its coordinates via WebSocket to the mandelbrot.frontend server

            var frontendImage = ImageViewModel.FromArraySegment<ImageViewModel>(clientImageMsg);
            Assert.That(frontendImage.Coordinates.Z, Is.EqualTo(1)); // received coordinates
            Assert.That(frontendImage.Main, Is.Not.Null); // Server side FSM in initial state already instantiated
            frontendImage.SaveMembers(); // special case when receiving a "flat" ViewModel: assign to the FSM
            Assert.That(frontendImage.Main.Coordinates.Z, Is.EqualTo(1));

            // Compute the image parameters to prepare the image for the renderer
            frontendImage.Main.Fsm.ComputeParams(new Resolution(16, 16));
            var parametrizedImageMsg = frontendImage.ToArraySegment();

            // --> Send the image with the parameters to the mandelbrot.renderer service which is unaware of coordinates

            var rendererImage = ImageViewModel.FromArraySegment<ImageViewModel>(parametrizedImageMsg);
            Assert.That(rendererImage.Main.Bytes.Length, Is.EqualTo(160)); // length of empty.png
            rendererImage.Main.AddInternalStateChangedHandler();    // not serialized, but needed for Render()
            rendererImage.Main.Render(); // skip callbacks and asynchronism here
            Assert.That(rendererImage.Main.Bytes.Length, Is.GreaterThan(160));
        }
    }
}