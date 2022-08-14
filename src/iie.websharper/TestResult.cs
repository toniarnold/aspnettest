using WebSharper.Sitelets;

namespace iie
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
                            using (var w = new System.IO.StreamWriter(stream, leaveOpen: true))
                            {
                                w.Write(TestRunner.StaticResultString);
                            }
                        }
                    )
                )
                .Install();
    }
}