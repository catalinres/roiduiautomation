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
    }
    public class TRHelper : TestRailClient
    {
        public TRHelper() : base(Globals.TRURL, Globals.TRUser, Globals.TRPass)
        {
        }
        public void RunAfterEach()
        {
            // TestRail stuff
            ulong TestProjectID = 1;
            ulong TestSuiteID = 1;
            ulong TestPlanID = 2;
            ulong TestAutomationSectionID = 1;
            //HashSet<ulong> caseIDsList = new HashSet<ulong> { 4, 8 };
            //List<ulong> caseIDsList = { 99, 98, 92, 97, 95 };

            //initialize TR
            string FeatureFilePath = Directory.GetCurrentDirectory() + @"\SeleniumSpecFlowTests\Tests\Features\ServiceTests.feature";

            //  Could not find a part of the path 'c:\SeleniumSpecFlowTests\Tests\Features\ServiceTests.feature'.--TearDown

            TestRailClient trail = new TestRailClient(Globals.TRURL, Globals.TRUser, Globals.TRPass);
            //Get current scenario name and tags and id
            var scenarioTitle = ScenarioContext.Current.ScenarioInfo.Title;
            var scenarioTags = ScenarioContext.Current.ScenarioInfo.Tags;
            var currentScenarioID = scenarioTitle.Split(' ')[0];
            //Get current scenario steps by parsing the feature file
            string scenarioText = "";
            Console.WriteLine("====BEFORE FILE READ");
            string[] lines = File.ReadAllLines(FeatureFilePath, System.Text.Encoding.UTF8);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(currentScenarioID))
                {
                    bool ScenarioEnd = false;
                    int j = i + 1;
                    while ((j < lines.Length) && (ScenarioEnd != true))
                    {
                        if (lines[j].Contains("Scenario:"))
                        {
                            ScenarioEnd = true;
                        }
                        else
                        {
                            scenarioText = scenarioText + lines[j] + System.Environment.NewLine;
                            Console.WriteLine("===============================" + scenarioText);
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
                Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-TRAIL CASE REFERENCES");
                Console.WriteLine(trail.GetCase(caseToUpdateID).References.ToString());

                //trail.UpdateCase(caseToUpdateID, scenarioTitle, null, null, null, null, scenarioText);

            }
            else
            {
                //                trail.AddCase(TestAutomationSectionID, scenarioTitle, null, null, null, null, null);

                JObject customs = new JObject(
                    new JProperty("custom_steps", scenarioText)
                 );

                _AddCase_(TestAutomationSectionID, scenarioTitle, null, null, null, null, null, customs);


            };

            //            //trail.AddRun(TestProjectID, TestSuiteID, "MyTestRun", "myTRDesc", 1, null, null);




            //-===================
            //trail.GetCases(TestProjectID, TestSuiteID, TestAutomationSectionID)[1].



            //trail.AddPlanEntry(TestPlanID, TestSuiteID, "TestRunCreatedByMe",null, caseIDsList);


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
