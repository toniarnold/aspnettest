using System.Web;
using WebSharper;
using WebSharper.Sitelets;
using WebSharper.UI;
using static WebSharper.UI.Html;
using WebSharper.JavaScript;

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
                            using (var w = new System.IO.StreamWriter(stream))
                            {
                                w.Write(TestRunner.StaticResultString);
                            }
                        }
                    )
                )
                .Install();
    }
}
