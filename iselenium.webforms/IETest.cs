using OpenQA.Selenium.IE;
using System;

namespace iselenium
{
    [Obsolete("Replaced by SeleniumTest<InternetExplorerDriver>")]
    public class IETest : SeleniumTestBase<InternetExplorerDriver>, ISelenium
    {
    }

    [Obsolete("Replaced by SeleniumTest<InternetExplorerDriver, TMain>")]
    public class IETest<TMain> : SeleniumTest<InternetExplorerDriver, TMain>, ISelenium
        where TMain : new()
    {
    }
}