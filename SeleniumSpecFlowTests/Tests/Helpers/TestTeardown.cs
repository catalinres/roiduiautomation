using OpenQA.Selenium;
using SeleniumSpecFlowTests.Pages.Common;
using SeleniumSpecFlowTests.Pages.Implementations;
using SeleniumSpecFlowTests.Tests.Helpers;
using TechTalk.SpecFlow;
using NUnit.Framework;
using System;
using System.IO;
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
            ulong TestProjectID = 1;
            ulong TestSuiteID = 1;
            ulong TestPlanID = 2;
            ulong TestAutomationSectionID = 1;
            //HashSet<ulong> caseIDsList = new HashSet<ulong> { 4, 8 };
            //List<ulong> caseIDsList = { 99, 98, 92, 97, 95 };

            //initialize TR
            string TRURL = "https://catalinres.testrail.net";
            string TRUser = "c.lungu@res.com";
            string TRPassword = "Resforever123!";
            string FeatureFilePath = "\\SeleniumSpecFlowTests\\Tests\\Features\\ServiceTests.feature";
            TestRailClient trail = new TestRailClient(TRURL, TRUser, TRPassword);
            //Get current scenario name and tags and id
            var scenarioTitle = ScenarioContext.Current.ScenarioInfo.Title;
            var scenarioTags = ScenarioContext.Current.ScenarioInfo.Tags;
            var currentScenarioID = scenarioTitle.Split(' ')[0];
            //Get current scenario steps by parsing the feature file
            string scenarioText = "";
            string[] lines = File.ReadAllLines(FeatureFilePath, System.Text.Encoding.UTF8); 
            for (int i=0; i<lines.Length;  i++)
            {
                if (lines[i].Contains(currentScenarioID))
                    {
                    bool ScenarioEnd = false;
                    int j = i+1;
                    while ((j<lines.Length) && (ScenarioEnd != true))
                    {
                        if (lines[j].Contains("Scenario:"))
                        {
                            ScenarioEnd = true;
                        }
                        else
                        {
                            scenarioText = scenarioText + lines[j] + System.Environment.NewLine;
                            j++;
                        };
                    };
                };
            };

            //get list of existing scenarios from TestRail and scan for current scenario
            List<TestRail.Types.Case> TRCasesList = trail.GetCases(TestProjectID, TestSuiteID, TestAutomationSectionID);
            int casestep = 0;
            ulong caseToUpdateID = 0;
            while ((casestep < TRCasesList.Count) && (caseToUpdateID != 0))
                {
                if (TRCasesList[casestep].Title.Contains(currentScenarioID))
                {
                    caseToUpdateID = TRCasesList[casestep].ID.Value;
                    
                };
                casestep++;
            };
            //if current scenario exists in TestRail, then update it, else create a new one
            if (caseToUpdateID != 0)
            {
                trail.UpdateCase(caseToUpdateID, scenarioTitle, null, null, null, null, scenarioText);
            }
            else
            {

            };



            //-===================
            //trail.GetCases(TestProjectID, TestSuiteID, TestAutomationSectionID)[1].



            //trail.AddPlanEntry(TestPlanID, TestSuiteID, "TestRunCreatedByMe",null, caseIDsList);

            trail.AddRun(TestProjectID, TestSuiteID, "MyTestRun", "myTRDesc", 1, null, null);

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
