using OpenQA.Selenium;

namespace SeleniumSpecFlowTests.Pages.Common
{
    public static class BottomBar
    {
        public static void ClickAdd()
        {
            WebDriver.Instance().FindElementWhenClickable(By.CssSelector("nav.bottomBar #btnAdd")).Click();
        }

        public static void ClickSave()
        {
            WebDriver.Instance().FindElementWhenClickable(By.CssSelector("nav.bottomBar .actionButton .fa-save")).Click();
        }

        public static void ClickDelete()
        {
            WebDriver.Instance().FindElementWhenClickable(By.CssSelector("nav.bottomBar .actionButton .fa-trash-o")).Click();
        }

        public static bool SaveButtonIsDisabled()
        {
            IWebElement saveIcon = WebDriver.Instance().FindElementWhenVisible(By.CssSelector("nav.bottomBar .actionButton .fa-save"));
            return WebDriver.Instance().NormalWait.Until(a =>
            {
                IWebElement saveButton = saveIcon.FindElement(By.XPath(".."));
                string classAttribute = saveButton.GetAttribute("class");
                return classAttribute.Contains("disabled");
            });
        }
    }
}
