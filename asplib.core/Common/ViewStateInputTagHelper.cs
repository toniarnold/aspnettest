using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Net;

namespace asplib.Common
{
    [HtmlTargetElement("input", Attributes="viewstate")]
    public class ViewStateInputTagHelper : TagHelper
    {
        public override async Task ProcessAsync(
            TagHelperContext context, TagHelperOutput output)
        {
            await output.GetChildContentAsync();

            // HtmlDecode to avoid <input viewstate="@Html.Raw(ViewBag.ViewState)" />
            var viewstate = WebUtility.HtmlDecode(context.AllAttributes["viewstate"].Value.ToString());
            if (!String.IsNullOrEmpty(viewstate))
            { 
                var namevalue = viewstate.Split(":");

                output.Attributes.Clear();
                output.Attributes.SetAttribute("type", "hidden");
                output.Attributes.SetAttribute("name", namevalue[0]);
                output.Attributes.SetAttribute("value", (namevalue[1]));
            }
            else
            {
                // ViewState declared, but not used -> defensively rewrite it as empty hidden input
                output.Attributes.Clear();  
                output.Attributes.SetAttribute("type", "hidden");
            }
        }
    }
}
