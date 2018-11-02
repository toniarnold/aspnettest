using System.Web.UI.HtmlControls;

namespace asplib.View
{
    /// <summary>
    /// Makeshift Share Button composed of 2 Unicode symbols
    /// </summary>
    public class ShareButton : HtmlAnchor
    {
        public ShareButton() : base()
        {
            this.SetUp();
        }

        private void SetUp()
        {
            this.Attributes.Add("style", "text-decoration: none;");
            this.InnerHtml = @"
<span style='font-size: 40px;'>
□
</span>
<span style='position: relative; font-size: 21px; left: -27px; bottom: +12px;'>
↑
</span>
";
        }
    }
}