using WebSharper;
using WebSharper.Sitelets;

namespace iselenium
{
    public class TestResultSite
    {
        public const string PathFailed = "/testresult/failed";  // used in JS.Window.Location.Assign

        [EndPoint("/testresult/{failed}")]
        public class ResultEndpoint
        {
            public string failed;
        }

        [Website]
        public static Sitelet<object> Main =>
            new SiteletBuilder()
                .With<ResultEndpoint>((ctx, endpoint) =>
                    Content.Custom(
                        Status: Http.Status.Ok,
                        Headers: new[] { Http.Header.Custom("Content-Type", "application/xml") },
                        WriteBody: stream =>
                        {
                            using (var w = new System.IO.StreamWriter(stream))
                            {
                                if (endpoint.failed == "failed")
                                {
                                    w.Write(TestRunner.ResultFailedXml);
                                }
                                else
                                {
                                    w.Write(TestRunner.ResultXml);
                                }
                            }
                        }
                    )
                )
                .Install();
    }
}