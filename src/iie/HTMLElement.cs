using MSHTML;

namespace iie
{
    /// <summary>
    /// IHTMLElement adapter to identically pass to the .NET Framework and
    /// .NET Core without referencing the incompatible COM assembly:
    /// Microsoft.mshtml.dll (Standard/Framework) != Interop.MSHTML.dll (Core)
    /// Interface calls yield HRESULT:0x80004002 (E_NOINTERFACE)
    /// </summary>
    public class HTMLElement
    {
        internal IHTMLElement IHTMLElement { get; }

        public HTMLElement(IHTMLElement concreteIHTMLElement)
        {
            this.IHTMLElement = concreteIHTMLElement;
        }

        public void click()
        {
            this.IHTMLElement.click();
        }

        public string getAttribute(string name, int flags = 0)
        {
            return (string)this.IHTMLElement.getAttribute(name, flags);
        }

        public void setAttribute(string name, string value, int flags = 0)
        {
            this.IHTMLElement.setAttribute(name, value, flags);
        }
    }
}