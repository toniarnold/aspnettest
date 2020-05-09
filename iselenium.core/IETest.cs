using OpenQA.Selenium.IE;
using System;

namespace iselenium
{
    [Obsolete("Replaced by SeleniumTest<InternetExplorerDriver>")]
    public class IETest : SeleniumTest<InternetExplorerDriver>
    {
    }

    [Obsolete("Replaced by SeleniumTest<InternetExplorerDriver, TController>")]
    public class IETest<TController> : SeleniumTest<InternetExplorerDriver, TController>
    {
    }
}