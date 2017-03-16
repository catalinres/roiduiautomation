using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace SeleniumSpecFlowTests.Pages.Implementations
{
    public class LoginPage : BasePage
    {
        private static bool _isLoggedIn;

        private IWebElement Username => Driver.FindElementWhenVisible(By.Id("loginUserName"));

        private IWebElement Password => Driver.FindElementWhenVisible(By.Id("loginPassword"));

        private IWebElement SignIn => Driver.FindElementWhenVisible(By.Id("loginBtn"));

        public void Login(string username, string password)
        {
            if (!_isLoggedIn)
            {
                Username.SendKeys(username);
                Password.SendKeys(password);
                SignIn.Click();

                _isLoggedIn = true;
            }
        }
    }
}
