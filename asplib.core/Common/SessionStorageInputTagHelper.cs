using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Net;

namespace asplib.Common
{
    [HtmlTargetElement("input", Attributes = "sessionstorage")]
    public class SessionStorageInputTagHelper : TagHelper
    {
        public override async Task ProcessAsync(
            TagHelperContext context, TagHelperOutput output)
        {
            await output.GetChildContentAsync();

            var sessionstorage = WebUtility.HtmlDecode(context.AllAttributes["sessionstorage"].Value.ToString());
            if (!String.IsNullOrEmpty(sessionstorage))
            {
                var namevalue = sessionstorage.Split(":");

                output.Attributes.Clear();
                output.Attributes.SetAttribute("type", "hidden");
                output.Attributes.SetAttribute("name", namevalue[0]);
                output.Attributes.SetAttribute("value", (namevalue[1]));
            }
            else
            {
                // SessionStorage declared, but not used -> defensively rewrite it as empty hidden input
                output.Attributes.Clear();
                output.Attributes.SetAttribute("type", "hidden");
            }
        }
    }
}
