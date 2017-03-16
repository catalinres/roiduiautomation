namespace SeleniumSpecFlowTests.Tests.Helpers
{
    public class TestTeardown
    {
        public static void Run()
        {
            // End browser session
            WebDriver.Instance().Quit();
        }
    }
}
