using OpenQA.Selenium;

namespace SeleniumSpecFlowTests.Pages.Implementations
{
    public class Menu : BasePage
    {
        const string idMenu = "side-menu";
        const string CatalogString = "Service Catalog";
        const string Home = "Dashboard";
        //private IWebElement ServiceCatalogMenuItem => Driver.FindElementWhenVisible(By.XPath($"//a[contains(text(), '{name}')]"));
        //private IWebElement ServiceCatalogMenuItem => Driver.FindElementWhenVisible(By.XPath($"//*[@id='{idMenu}']/li[2]/a"));
        public static void GoToServiceCatalog()
        {
            //ServiceCatalogMenuItem.Click();
            WebDriver.Instance().FindElementWhenClickable(By.XPath($"//a[contains(text(), '{CatalogString}')]")).Click();
        }
        public static void GoToHomePage()
        {
            //ServiceCatalogMenuItem.Click();
            WebDriver.Instance().FindElementWhenClickable(By.XPath($"//a[contains(text(), '{Home}')]")).Click();
        }

    }
}