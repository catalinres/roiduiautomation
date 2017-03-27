using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework; //added instead of microsoft framework

namespace SeleniumSpecFlowTests
{
    public class WebDriver : ChromeDriver
    {
        const string ManagementPortalUrl = "https://cl82001/identitydirector";

        public WebDriverWait ShortWait => new WebDriverWait(this, TimeSpan.FromSeconds(2));
        public WebDriverWait NormalWait => new WebDriverWait(this, TimeSpan.FromSeconds(5));
        public WebDriverWait LongWait => new WebDriverWait(this, TimeSpan.FromSeconds(10));

        private static ChromeOptions Options
        {
            get
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("no-sandbox");
                return options;
            }
        }

        private static WebDriver _instance;

        private WebDriver() : base(@"C:\chdriver", Options)
        {

        }

        private void Initialize()
        {
            Navigate().GoToUrl(ManagementPortalUrl);
        }

        public static WebDriver Instance()
        {
            if (_instance == null)
            {
                _instance = new WebDriver();
                _instance.Initialize();
            }
            return _instance;
        }

        public IWebElement FindElementWhenVisible(By @by)
        {
            try
            {
                return NormalWait.Until(ExpectedConditions.ElementIsVisible(by));
            }
            catch (WebDriverTimeoutException ex)
            {
                Assert.Fail($"Failed to find element '{by}'. {ex.Message}");
            }
            return null;
        }

        public IWebElement FindElementWhenClickable(By @by)
        {
            try
            {
                return NormalWait.Until(ExpectedConditions.ElementToBeClickable(by));
            }
            catch (WebDriverTimeoutException ex)
            {
                Assert.Fail($"Failed to find element '{by}'. {ex.Message}");
            }
            return null;
        }

        public bool UrlContains(string fraction)
        {
            return ShortWait.Until(ExpectedConditions.UrlContains(fraction));
        }

        public bool UrlContainsValidGuid()
        {
            return ShortWait.Until(ExpectedConditions.UrlMatches(@"([0-9A-Fa-f]){8}-([0-9A-Fa-f]){4}-([0-9A-Fa-f]){4}-([0-9A-Fa-f]){4}-([0-9A-Fa-f]){12}"));
        }

        public bool WaitForRedirect()
        {
            var currentUrl = Url;
            return LongWait.Until(a => !a.Url.Equals(currentUrl));
        }
    }
}
