using System.Web;
using WebSharper;
using WebSharper.Sitelets;
using WebSharper.UI;
using static WebSharper.UI.Html;
using WebSharper.JavaScript;

namespace iie.websharper
{
    public class TestResult
    {
        [Website]
        public static Sitelet<object> Main =>
            new SiteletBuilder()
                .With("/testresult", ctx =>
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
