using FractalSharp.Algorithms;
using FractalSharp.Algorithms.Coloring;
using FractalSharp.Algorithms.Fractals;
using FractalSharp.Imaging;
using FractalSharp.Processing;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace mandelbrot.image
{
    /// <summary>
    /// Transient Image processor modeled after FractalSharp.ExampleApp,
    /// internal to mandelbrot.image
    /// </summary>
    internal class Renderer
    {
        private readonly FractalProcessor<double, SquareMandelbrotAlgorithm<double>> _fractalProcessor;
        private readonly ColorProcessor<SmoothColoringAlgorithm> _outerColorProcessor;
        private readonly ColorProcessor<RadialGradientAlgorithm> _innerColorProcessor;
        private readonly SkiaImageBuilder _imager;
        private readonly Gradient _colors;
        private readonly EscapeTimeParams<double> _params;
        private PointData[,] _fractal;
        private double[,] _innerIndicies;
        private double[,] _outerIndicies;

#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] Image { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        public Renderer(EscapeTimeParams<double> algorithmParams, int width, int height)
        {
            _fractalProcessor = new FractalProcessor<double, SquareMandelbrotAlgorithm<double>>(width, height);
            _outerColorProcessor = new ColorProcessor<SmoothColoringAlgorithm>(width, height);
            _innerColorProcessor = new ColorProcessor<RadialGradientAlgorithm>(width, height);
            _imager = new SkiaImageBuilder();
            _colors = new Gradient(256, new List<GradientKey>
            {
                new GradientKey(new RgbaValue(9, 1, 47)),
                new GradientKey(new RgbaValue(4, 4, 73)),
                new GradientKey(new RgbaValue(0, 7, 100)),
                new GradientKey(new RgbaValue(12, 44, 138)),
                new GradientKey(new RgbaValue(24, 82, 177)),
                new GradientKey(new RgbaValue(57, 125, 209)),
                new GradientKey(new RgbaValue(134, 181, 229)),
                new GradientKey(new RgbaValue(211, 236, 248)),
                new GradientKey(new RgbaValue(241, 233, 191)),
                new GradientKey(new RgbaValue(248, 201, 95)),
                new GradientKey(new RgbaValue(255, 170, 0)),
                new GradientKey(new RgbaValue(204, 128, 0)),
                new GradientKey(new RgbaValue(153, 87, 0)),
                new GradientKey(new RgbaValue(106, 52, 3)),
                new GradientKey(new RgbaValue(66, 30, 15)),
                new GradientKey(new RgbaValue(25, 7, 26))
            });
            _params = algorithmParams;
        }

        // Synchronized atomic FSM Actions

        public void ComputeFractal()
        {
            Task.Run(async () =>
            {
                await _fractalProcessor.SetupAsync(new ProcessorConfig
                {
                    ThreadCount = Environment.ProcessorCount,
                    Params = _params
                }, CancellationToken.None);
                _fractal = await _fractalProcessor.ProcessAsync(CancellationToken.None);
            }).Wait();
        }

        public void ComputeInnerColors()
        {
            Task.Run(async () =>
            {
                await _innerColorProcessor.SetupAsync(new ColorProcessorConfig
                {
                    ThreadCount = Environment.ProcessorCount,
                    Params = new RadialGradientParams
                    {
                        Scale = 256
                    },
                    PointClass = PointClass.Inner,
                    InputData = _fractal
                }, CancellationToken.None);
                _innerIndicies = await _innerColorProcessor.ProcessAsync(CancellationToken.None);
            }).Wait();
        }

        public void ComputeOuterColors()
        {
            Task.Run(async () =>
            {
                await _outerColorProcessor.SetupAsync(new ColorProcessorConfig
                {
                    ThreadCount = Environment.ProcessorCount,
                    Params = new EmptyColoringParams(),
                    PointClass = PointClass.Outer,
                    InputData = _fractal
                }, CancellationToken.None);
                _outerIndicies = await _outerColorProcessor.ProcessAsync(CancellationToken.None);
            }).Wait();
        }

        public void CreateImage()
        {
            _imager.CreateImage(_outerIndicies, _innerIndicies, _colors, _colors);
        }

        public void EncodeImage()
        {
            using var stream = new SKDynamicMemoryWStream();
            _imager.Bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            Image = stream.DetachAsData().ToArray();
        }
    }
}