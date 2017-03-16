using OpenQA.Selenium;

namespace SeleniumSpecFlowTests.Pages.Implementations
{
    public class ServiceCatalogPage : BasePage
    {
        private IWebElement NameField => Driver.FindElementWhenVisible(By.Id("name"));

        public void NavigateToServiceCatalogPage()
        {
            Driver.FindElementWhenVisible(By.CssSelector("body > nav.topBar .topBarLeft")).Click();
            Driver.FindElementWhenVisible(By.CssSelector("body > nav.topBar .topBarLeft .dropdown-menu li:nth-child(3)")).Click();
        }

        public void FillInNameField(string name)
        {
            NameField.SendKeys(name);
        }

        public void OpenServiceFromOverviewPage(string name)
        {
            IWebElement element = WebDriver.Instance().FindElementWhenVisible(By.XPath($"//a[contains(text(), '{name}')]"));
            ScrollIntoView(element);
            element.Click();
        }

    }
}
