using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace asplib.Common
{
    [HtmlTargetElement("button", Attributes = "share")]
    public class ShareButtonTagHelper : TagHelper
    {
        public override async Task ProcessAsync(
             TagHelperContext context, TagHelperOutput output)
        {
            await output.GetChildContentAsync();

            output.Attributes.RemoveAll("share");
            output.Attributes.SetAttribute("style", "border: none; padding: 0; background: none;");
            output.Content.AppendHtml(@"
<span style='font-size: 40px;'>
□
</span>
<span style='position: relative; font-size: 21px; left: -28px; bottom: +12px;'>
↑
</span>
");
        }
    }
}