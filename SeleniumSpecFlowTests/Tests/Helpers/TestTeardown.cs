using OpenQA.Selenium;
using SeleniumSpecFlowTests.Pages.Common;
using SeleniumSpecFlowTests.Pages.Implementations;
using SeleniumSpecFlowTests.Tests.Helpers;
using TechTalk.SpecFlow;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TestRail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SeleniumSpecFlowTests.Tests.Helpers
{
    public class TestTeardown
    {
        public static void Run()
        {
            // End browser session
            WebDriver.Instance().Quit();
        }

        public static void RunAfterEach()
        {
            // TestRail stuff
            //ulong TestProjectID = 1;
            ulong TestSuiteID = 1;
            ulong TestPlanID = 2;
            List<ulong> caseIDsList = new List<ulong> { 4, 8 };
            //List ulong caseIDsList = { 99, 98, 92, 97, 95 };
            string TRURL = "https://catalinres.testrail.net";
            string TRUser = "c.lungu@res.com";
            string TRPassword = "Resforever123!";
            //initialize TR
            TestRailClient trail = new TestRailClient(TRURL,TRUser,TRPassword);
            trail.AddPlanEntry(TestPlanID, TestSuiteID, "TestRunCreatedByMe",null,caseIDsList);
            //Get current scenario name

            //Get current scenario status (P/F)

            //Get error on failed tests:
                //if (ScenarioContext.Current.TestError != null)
                //{
                //    var error = ScenarioContext.Current.TestError;
                //    string errMsg = error.Message;
                //    string errType = error.GetType().Name;
                //}

        }

    }
}
