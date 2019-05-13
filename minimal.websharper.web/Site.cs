using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using WebSharper;
using WebSharper.Sitelets;
using WebSharper.UI;
using static WebSharper.UI.Html;
using asplib.websharper;
using WebSharper.JavaScript;

namespace minimal.websharper.web
{
    public class Site
    {
        [EndPoint("/")]
        public class Home {
            public override bool Equals(object obj) => obj is Home;
            public override int GetHashCode() => 0;
        }

        [EndPoint("GET /withstatic")]
        public class WithStatic
        {
            public override bool Equals(object obj) => obj is WithStatic;
            public override int GetHashCode() => 1;
        }

        [JavaScript]
        public static async Task Test()
        {
            await Remoting.Test();
        }

        static Var<string> testResult = Var.Create("");

        public static Task<Content> Page(Context<object> ctx, object endpoint, string title, Doc body) =>
            Content.Page(
                new Template.Main()
                    .Title(title)
                    .Test(async (el, ev) => {
                        // Blocked by dotnet-websharper/ui#191
                        testResult.Value = await Remoting.Test();
                    })
                    .TestResult(testResult.View)
                    .Doc()
            );

        [Website]
        public static Sitelet<object> Main =>
            new SiteletBuilder()
                .With<Home>((ctx, action) =>
                    Page(ctx, action, "minimal websharper web",
                        doc()
                    )
                )
                .Install();
    }
}