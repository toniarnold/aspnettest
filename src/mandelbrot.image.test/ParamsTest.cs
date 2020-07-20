using asplib.Model;
using NUnit.Framework;

namespace mandelbrot.image.test
{
    public class ParamsTest
    {
        // The example used a constant 128
        [Test]
        public void MaxIterationsTest()
        {
            Params m1 = new Params(1, new Complex(0, 0), 4);
            Assert.That(m1.MaxIterations, Is.EqualTo(100));

            Params m10 = new Params(10, new Complex(0, 0), 4);
            Assert.That(m10.MaxIterations, Is.EqualTo(192));

            Params m100 = new Params(100, new Complex(0, 0), 4);
            Assert.That(m100.MaxIterations, Is.EqualTo(284));

            Params pe6 = new Params(1e+6, new Complex(0, 0), 4);
            Assert.That(pe6.MaxIterations, Is.EqualTo(652));

            Params me12 = new Params(1e+12, new Complex(0, 0), 4);
            Assert.That(me12.MaxIterations, Is.EqualTo(1205));

            // zoom out (magnification < 1) is a pathological case, threat as 1
            Params m01 = new Params(0.1, new Complex(0, 0), 4);
            Assert.That(m01.MaxIterations, Is.EqualTo(100));
        }

        [Test]
        public void JsonSerializationTest() // not that natural with F# default immutability
        {
            var param = new Params(1, new Complex(2, 3), 4);
            var json = Json.Serialize(param);
            Assert.That(json, Does.Contain("magnification"));
            var copy = Json.Deserialize<Params>(json);
            Assert.That(copy.Magnification, Is.EqualTo(1.0));
            Assert.That(copy.Location.Real, Is.EqualTo(2.0));
            Assert.That(copy.Location.Imag, Is.EqualTo(3.0));
            Assert.That(copy.EscapeRadius, Is.EqualTo(4.0));
        }
    }
}