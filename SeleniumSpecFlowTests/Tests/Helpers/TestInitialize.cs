using SeleniumSpecFlowTests.Pages.Implementations;

namespace SeleniumSpecFlowTests.Tests.Helpers
{
    public class TestInitialize
    {
        public static void Run()
        {
            // Login
            LoginPage page = new LoginPage();
            page.Login(@"resqa\administrator", "reverofser");
        }
    }
}
