﻿using OpenQA.Selenium;

namespace SeleniumSpecFlowTests.Pages.Implementations
{
    public class HomePage : BasePage
    {
        private IWebElement ServiceCatalogMenuItem => Driver.FindElementWhenVisible(By.ClassName("fa fa-shopping-cart fa-fw"));
        //<i class="fa fa-shopping-cart fa-fw"></i>
        //     public void NavigateToServiceCatalogPage()
        //     {
        //         Driver.FindElementWhenVisible(By.CssSelector("body > nav.topBar .topBarLeft")).Click();
        //         Driver.FindElementWhenVisible(By.CssSelector("body > nav.topBar .topBarLeft .dropdown-menu li:nth-child(3)")).Click();
        //     }

        public void GoToServiceCatalog()
        {
            ServiceCatalogMenuItem.Click();
        }

        public void OpenServiceFromOverviewPage(string name)
        {
            IWebElement element = WebDriver.Instance().FindElementWhenVisible(By.XPath($"//a[contains(text(), '{name}')]"));
            ScrollIntoView(element);
            element.Click();
        }

    }
}
