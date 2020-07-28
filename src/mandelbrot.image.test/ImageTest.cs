using asplib.Model;
using FractalSharp.Algorithms.Fractals;
using FractalSharp.Numerics.Generic;
using NUnit.Framework;
using statemap;
using System;
using System.IO;
using static ImageContext.RenderMap;

namespace mandelbrot.image.test
{
    public class ImageTest
    {
        /*
         * FractalSharp.ExampleApp:
                Params = new EscapeTimeParams<double>
                {
                    MaxIterations = 256,
                    Magnification = 1.0,
                    Location = Complex<double>.Zero,
                    EscapeRadius = 4.0,
                }
         */

        private Image NewParametrizedImage(Resolution resolution)
        {
            var image = new Image(new Params(1.0, new Complex(-0.7, 0.0), 4.0), resolution);
            return image;
        }

        [Test]
        public void InitTest()
        {
            var image = new Image();
            Assert.That(image.ReachedState, Is.EqualTo(Empty));
            Assert.That(image.IsEmpty);
        }

        [Test]
        public void EmptyImageTest()
        {
            var image = new Image(); // without cache
            var empty = image.Bytes;
            Assert.That(empty.Length, Is.EqualTo(160)); // length of empty.png
        }

        [Test]
        [Ignore("wrong assumptions")]
        public void ComputeParams0Test()
        {
            var coord = new Coordinates(0, 0, 0);
            var image = new Image(coord);
            image.ComputeParams(new Resolution(0, 0));
            Assert.Multiple(() =>
            {
                Assert.That(image.Params, Is.EqualTo(image.Specification.Params));
                Assert.That(image.Params.Magnification, Is.EqualTo(1.0));
                Assert.That(image.Params.Location.Real, Is.EqualTo(0.0));
                Assert.That(image.Params.Location.Imag, Is.EqualTo(0.0));
            });
        }

        [Test]
        [Ignore("wrong assumptions")]
        public void ComputeParams1Test()
        {
            // Z=1 -> zoom 1 denotes a Zero grid -1 .. +1 with 1 tile of size 2
            var coord = new Coordinates(1, 0, 1);
            var image = new Image(coord);
            image.ComputeParams(new Resolution(0, 0));
            Assert.Multiple(() =>
            {
                Assert.That(image.Params.Magnification, Is.EqualTo(3.0));
                Assert.That(image.Params.Location.Real, Is.EqualTo(2.0 / 3));
                Assert.That(image.Params.Location.Imag, Is.EqualTo(0));
            });
        }

        [Test]
        [Ignore("wrong assumptions")]
        public void ComputeParams2Test()
        {
            // Z=2 -> zoom 5 denotes a Zero grid -1 .. +1 with 5 tiles of size 2/5
            var coord = new Coordinates(2, 1, 2);
            var image = new Image(coord);
            image.ComputeParams(new Resolution(0, 0));
            Assert.Multiple(() =>
            {
                Assert.That(image.Params.Magnification, Is.EqualTo(5.0));
                Assert.That(image.Params.Location.Real, Is.EqualTo(0.8));
                Assert.That(image.Params.Location.Imag, Is.EqualTo(0.4));
            });
        }

        [Test]
        [Ignore("wrong assumptions")]
        public void ComputeParams_1Test()
        {
            //  Z=-1 -> zoom 0.3333 denotes a grid with tiles of size 6 (2*3)
            var coord = new Coordinates(1, 2, -1);
            var image = new Image(coord);
            image.ComputeParams(new Resolution(0, 0));
            Assert.Multiple(() =>
            {
                Assert.That(image.Params.Magnification, Is.EqualTo(0.3333).Within(1E-4));
                Assert.That(image.Params.Location.Real, Is.EqualTo(6.0));
                Assert.That(image.Params.Location.Imag, Is.EqualTo(12.0));
            });
        }

        [Test]
        [Ignore("wrong assumptions")]
        public void ComputeParams_2Test()
        {
            // Z=-2 ->  zoom 0.2 denotes a grid width tiles of size 10 (2*5)
            var coord = new Coordinates(1, 2, -2);
            var image = new Image(coord);
            image.ComputeParams(new Resolution(0, 0));
            Assert.Multiple(() =>
            {
                Assert.That(image.Params.Magnification, Is.EqualTo(0.2));
                Assert.That(image.Params.Location.Real, Is.EqualTo(10.0));
                Assert.That(image.Params.Location.Imag, Is.EqualTo(20.0));
            });
        }

        [Test]
        public void RenderTest()
        {
            var image = NewParametrizedImage(new Resolution(320, 240));
            image.AddStateChangedHandler(WriteStateChanged);
            image.Render();

            // To visualize the result:
            var filename = Path.GetFullPath(
                Path.Combine(TestContext.CurrentContext.WorkDirectory,
                    "..", "..", "..", "img", "mandelbrot.png"));
            using var stream = File.Create(filename);
            stream.Write(image.Bytes);
        }

        /// <summary>
        /// State Changed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void WriteStateChanged(object sender, StateChangeEventArgs args)
        {
            TestContext.Progress.WriteLine("FSM " +
                              args.FSMName() +
                              " " +
                              args.TransitionType() +
                              " transition from " +
                              args.PreviousState() +
                              " to " +
                              args.NewState() +
                              ".");
        }

        [Test]
        public void SizeTest()
        {
            var image = NewParametrizedImage(new Resolution(320, 240));
            var emptySize = image.Size();
            image.Render();
            var renderedSize = image.Size();
            Assert.That(renderedSize, Is.GreaterThan(emptySize));
            TestContext.Progress.WriteLine(String.Format("Size empty: {0}, Size rendered: {1}",
                                                            emptySize, renderedSize));
        }

        // Serialize/Deserialize might throw runtime exceptions...

        [Test]
        public void SerializeDeserializeEmptyTest()
        {
            var empty = new Image(new Coordinates());
            Assert.That(empty.Guid, Is.Not.EqualTo(Guid.Empty));
            empty.ComputeParams(new Resolution(16, 16));
            var bytes = Serialization.Serialize(empty);
            var copy = Serialization.Deserialize<Image>(bytes);
            Assert.That(copy.ReachedState, Is.EqualTo(Empty));
            Assert.That(copy.Guid, Is.EqualTo(empty.Guid));
            AssertParams(copy.Params, empty.Params);
        }

        [Test]
        public void SerializeDeserializeReadyTest()
        {
            var rendered = NewParametrizedImage(new Resolution(16, 16));
            rendered.Render();
            var bytes = Serialization.Serialize(rendered);
            var copy = Serialization.Deserialize<Image>(bytes);
            Assert.That(copy.ReachedState, Is.EqualTo(Ready));
            AssertParams(rendered.Params, copy.Params);
        }

        // ...and it in fact initially threw without the Params DTO for EscapeTimeParams<double>...

        [Test]
        public void SerializeParamsDirectlyThrowsTest()
        {
            // This was the culprit:
            var parameters = new EscapeTimeParams<double>()
            {
                MaxIterations = 256,
                Magnification = 1.0,
                Location = new Complex<double>(-0.7, 0),
                EscapeRadius = 4.0,
            };

            Assert.That(() => { Serialization.Serialize(parameters); }, Throws.Exception);
        }

        // ...after a failed quick attempt with Json.Map() finally a manual implementation of .Params

        [Test]
        public void EscapeTimeParamsAccessorTest()
        {
            var empty = NewParametrizedImage(new Resolution(16, 16));
            Assert.That(empty.EscapeTimeParams.EscapeRadius.Value, Is.EqualTo(4.0));
        }

        private void AssertParams(Params orig, Params copy)
        {
            Assert.Multiple(() =>
            {
                Assert.That(copy.MaxIterations, Is.EqualTo(orig.MaxIterations));
                Assert.That(copy.Magnification, Is.EqualTo(orig.Magnification));
                Assert.That(copy.Location.Real, Is.EqualTo(orig.Location.Real));
                Assert.That(copy.Location.Imag, Is.EqualTo(orig.Location.Imag));
                Assert.That(copy.EscapeRadius, Is.EqualTo(orig.EscapeRadius));
            });
        }
    }
}