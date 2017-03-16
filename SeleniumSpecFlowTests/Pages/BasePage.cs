using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace SeleniumSpecFlowTests.Pages
{
    public class BasePage : IPage
    {
        protected WebDriver Driver => WebDriver.Instance();

        public BasePage()
        {
            PageFactory.InitElements(Driver, this);
        }

        public void ScrollIntoView(IWebElement element)
        {
            (Driver as IJavaScriptExecutor).ExecuteScript($"window.scrollTo(0, {element.Location.Y + 200})");
        }
    }
}
