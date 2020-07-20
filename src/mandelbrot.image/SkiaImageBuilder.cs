using FractalSharp.Imaging;
using SkiaSharp;

namespace mandelbrot.image
{
    public class SkiaImageBuilder : ImageBuilder
    {
        public SKBitmap Bitmap { get; private set; }

        public override void InitializeImage(int width, int height)
        {
            Bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul));
        }

        public override void WritePixel(int x, int y, RgbaValue color)
        {
            Bitmap.SetPixel(x, y, new SKColor(color.Red, color.Green, color.Blue, color.Alpha));
        }
    }
}