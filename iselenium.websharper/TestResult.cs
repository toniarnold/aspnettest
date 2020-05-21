using WebSharper.Sitelets;

namespace iselenium
{
    public class TestResult
    {
        public const string Path = "/testresult";  // used in JS.Window.Location.Assign

        [Website]
        public static Sitelet<object> Main =>
            new SiteletBuilder()
                .With(Path, ctx =>
                    Content.Custom(
                        Status: Http.Status.Ok,
                        Headers: new[] { Http.Header.Custom("Content-Type", "application/xml") },
                        WriteBody: stream =>
                        {
                            using (var w = new System.IO.StreamWriter(stream))
                            {
#pragma warning disable CS0103 // Der Name "TestRunner" ist im aktuellen Kontext nicht vorhanden.
                                w.Write(TestRunner.ResultXml);
#pragma warning restore CS0103 // Der Name "TestRunner" ist im aktuellen Kontext nicht vorhanden.
                            }
                        }
                    )
                )
                .Install();
    }
}