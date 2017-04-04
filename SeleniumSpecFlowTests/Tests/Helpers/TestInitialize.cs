using SeleniumSpecFlowTests.Pages.Implementations;
using System;

namespace SeleniumSpecFlowTests.Tests.Helpers
{
    static class Globals
    {
        public static string ManagementPortalUrl = "https://cl82001/identitydirector";
        public static string TRURL = "https://catalinres.testrail.net";
        public static string TRUser = "c.lungu@res.com";
        public static string TRPass = "Resforever123!";
        public static string TR_RUN_NAME = Environment.GetEnvironmentVariable("TR_RUN_NAME");
        public static string TRRunName; //this will not be used if the env variable TR_RUN_NAME exists and has a value
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
