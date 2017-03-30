using SeleniumSpecFlowTests.Pages.Implementations;

namespace SeleniumSpecFlowTests.Tests.Helpers
{
    static class Globals
    {
        public static string TRURL = "https://catalinres.testrail.net";
        public static string TRUser = "c.lungu@res.com";
        public static string TRPass = "Resforever123!";
    }

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
