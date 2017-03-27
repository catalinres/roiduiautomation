//using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using SeleniumSpecFlowTests.Pages.Common;
using SeleniumSpecFlowTests.Pages.Implementations;
using SeleniumSpecFlowTests.Tests.Helpers;
using TechTalk.SpecFlow;
using NUnit.Framework; //added instead of microsoft framework

namespace SeleniumSpecFlowTests.Tests.Steps
{
    [Binding]
    public class ServiceTestsSteps
    {
        [Given(@"I navigate to Service Catalog")]
        public void INavigateToServiceCatalog()
        {
            Menu.GoToServiceCatalog();
        }

        [Then(@"I navigate to Home Page")]
        public void INavigateToHomePage()
        {
            Menu.GoToHomePage();
        }

        [Given(@"I fill in '(.*)' in the Name field")]
        public void GivenIFillInInTheNameField(string name)
        {
            ServiceCatalogPage page = new ServiceCatalogPage();
            page.FillInNameField(name);
        }

        [Given(@"I click Add in the bottom bar")]
        public void GivenIClickAddInTheBottomBar()
        {
            BottomBar.ClickAdd();
        }
        
        [When(@"I click Save in the bottom bar")]
        public void WhenIClickSaveInTheBottomBar()
        {
            BottomBar.ClickSave();
        }


        [Then(@"The address should contain '(.*)'")]
        public void ThenTheAddressShouldContain(string substring)
        {
            Assert.IsTrue(WebDriver.Instance().UrlContains(substring));
        }

        [Then(@"The address should not contain '(.*)'")]
        public void ThenTheAddressShouldNotContain(string substring)
        {
            try
            {
                WebDriver.Instance().UrlContains(substring);
                Assert.Fail("Expected WebDriverTimeoutException");
            }
            catch (WebDriverTimeoutException)
            {

            }
        }

        [Then(@"The address should contain a valid Guid")]
        public void ThenTheAddressShouldContainAValidGuid()
        {
            Assert.IsTrue(WebDriver.Instance().UrlContainsValidGuid());
        }

        [Then(@"The address should not contain a valid Guid")]
        public void ThenTheAddressShouldNotContainAValidGuid()
        {
            try
            {
                WebDriver.Instance().UrlContainsValidGuid();
                Assert.Fail("Expected WebDriverTimeoutException");
            }
            catch (WebDriverTimeoutException)
            {
                
            }
        }

        [Given(@"I click on the service '(.*)'")]
        public void GivenIClickOnTheService(string name)
        {
            ServiceCatalogPage page = new ServiceCatalogPage();
            page.OpenServiceFromOverviewPage(name);
        }

        [Given(@"I click Delete in the bottom bar")]
        public void GivenIClickDeleteInTheBottomBar()
        {
            BottomBar.ClickDelete();
        }

        [Given(@"I wait until the page is redirected")]
        [When(@"I wait until the page is redirected")]
        public void GivenIWaitUntilThePageIsRedirected()
        {
            WebDriver.Instance().WaitForRedirect();
        }

        [Then(@"The Save button in the bottom bar should be disabled")]
        public void ThenTheSaveButtonInTheBottomBarShouldBeDisabled()
        {
            Assert.IsTrue(BottomBar.SaveButtonIsDisabled());
        }

        //[AfterScenario]
        //protected static void AfterEachScenario()
        //{
        //    TestTeardown.RunAfterEach();
        //}

        [BeforeTestRun]
        protected static void Before()
        {
            TestInitialize.Run();
        }

        [AfterTestRun]
        protected static void After()
        {
            TestTeardown.Run();
            //TestTeardown.RunAfterEach();
        }
    }
}
