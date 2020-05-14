using OpenQA.Selenium.IE;
using System;

namespace iselenium
{
    [Obsolete("Replaced by SeleniumTest<InternetExplorerDriver>")]
    public class IETest : SeleniumTestBase<InternetExplorerDriver>, IIE, ISelenium
    {
    }

    [Obsolete("Replaced by SeleniumTest<InternetExplorerDriver, TMain>")]
    public class IETest<TMain> : SeleniumTest<InternetExplorerDriver, TMain>, IIE, ISelenium
        where TMain : new()
    {
    }
}