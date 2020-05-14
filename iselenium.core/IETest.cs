using OpenQA.Selenium.IE;
using System;

namespace iselenium
{
    [Obsolete("Replaced by SeleniumTest<InternetExplorerDriver>")]
    public class IETest : SeleniumTestBase<InternetExplorerDriver>, IIE, ISelenium
    {
    }

    [Obsolete("Replaced by SeleniumTest<InternetExplorerDriver, TController>")]
    public class IETest<TController> : SeleniumTest<InternetExplorerDriver, TController>, IIE, ISelenium
    {
    }
}