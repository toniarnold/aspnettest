using System.IO;
using WebSharper;
using WebSharper.Sitelets;

namespace iselenium
{
    public class TestResultSite
    {
        public const string ResultXmlPath = "/testresult";  // used in JS.Window.Location.Assign

        [EndPoint(ResultXmlPath)]
        public class ResultEndpoint
        {
        }

        [Website]
        public static Sitelet<object> Main =>
            new SiteletBuilder()
                .With<ResultEndpoint>((ctx, endpoint) =>
                    Content.Custom(
                        Status: Http.Status.Ok,
                        Headers: new[] { Http.Header.Custom("Content-Type", "application/xml; charset=UTF-8") },
                        WriteBody: stream =>
                        {
                            using (var writer = new StreamWriter(stream))
                            {
                                writer.Write(TestRunner.ResultXml);
                            }
                        }
                    )
                )
                .Install();
    }
}