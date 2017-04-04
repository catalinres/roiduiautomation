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
            //initialize TR
            string FeatureFilePath = Directory.GetCurrentDirectory() + @"\SeleniumSpecFlowTests\Tests\Features\ServiceTests.feature";
            TestRailClient trail = new TestRailClient(Globals.TRURL, Globals.TRUser, Globals.TRPass);
            //Get current scenario name and tags and id
            var scenarioTitle = ScenarioContext.Current.ScenarioInfo.Title;
            var scenarioTags = ScenarioContext.Current.ScenarioInfo.Tags;
            var currentScenarioID = scenarioTitle.Split(' ')[0];
            //Get current scenario steps by parsing the feature file
            string scenarioText = "";
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
            //place scenario body in custom field - JSON:
            JObject customs = new JObject(
                new JProperty("custom_steps", scenarioText)
            );
            //if current scenario exists in TestRail, then update it, else create a new one
            if (caseToUpdateID != 0)
            {
                _UpdateCase_(caseToUpdateID, scenarioTitle, null, null, null, null, null, customs);
            }
            else
            {
                _AddCase_(TestAutomationSectionID, scenarioTitle, null, null, null, null, null, customs);
        //retrieve id of currently added test -> use the loop to make sure no others were added meanwhile :)
                TRCasesList = trail.GetCases(TestProjectID, TestSuiteID, TestAutomationSectionID);
                casestep = 0;
                while ((casestep < TRCasesList.Count) && (caseToUpdateID != 0))
                {
                    if (TRCasesList[casestep].Title.Contains(currentScenarioID))
                    {
                        caseToUpdateID = TRCasesList[casestep].ID.Value;

                    };
                    casestep++;
                };
                if (caseToUpdateID == 0)
                {
                    Console.WriteLine("!!!!!!!! ========= SOMEHOW THE TEST CASE WAS NOT ADDED AND CASE ID IS NULL SO CASE WON'T BE ADDED TO TEST RUN ========== !!!!!!!");
                }
            };
            //set current test run name based on ENV variable
            if (Globals.TR_RUN_NAME != null)
            {
                Globals.TRRunName = Globals.TR_RUN_NAME;
            }
            else
            {
                Globals.TRRunName = DateTime.Now.ToShortDateString() + " - Automated Run"; //this will not be used if the env variable TR_RUN_NAME exists and has a value
            }
            //If current test run does not exist, create it and get the ID, else get ID from existing one
            List<TestRail.Types.Run> Test_Runs = trail.GetRuns(TestProjectID);
            ulong runID = 0;
            for (int i = 0; i < Test_Runs.Count; i++)
            {
                if (Test_Runs[i].Name == Globals.TRRunName)
                {
                    runID = Test_Runs[i].ID.Value;
                }
            }
            if (runID !=0) //run exists so let's add the scenario to it and also add the result to the scenario
            {
                //update existing test run -> add current scenario if not in there already
                List<TestRail.Types.Test> CasesList = trail.GetTests(runID);
                ulong TCaseID = 0;
                for (int i = 0; i < CasesList.Count; i++)
                {
                    if (CasesList[i].Title.Contains(currentScenarioID))
                    {
                        TCaseID = CasesList[i].ID.Value;
                    }
                }
                if (TCaseID !=0)  //test case was already added to current test run so we are updating its run result
                {
                    //Passed = 1,
                    //Blocked = 2,
                    //Untested = 3,
                    //Retest = 4,
                    //Failed = 5,
                    trail.AddResultForCase(runID, TCaseID, TestRail.Types.ResultStatus.Passed, "Test Run Result added automatically", null, null, null, null, null);
                }
                else  //test case was not added to test run so we are adding it to the test run and setting its run result
                {
//!                    TestRail.Types.Case newCase = trail.GetCase(caseToUpdateID);
                    HashSet<ulong> newCaseIDs = new HashSet<ulong>();
//                    List<ulong> newCaseIDs = new List<ulong>();
                    newCaseIDs.Add(caseToUpdateID);
//                    trail.UpdatePlanEntry(TestPlanID, runID.ToString(), Globals.TRRunName, null, newCaseIDs);
                    trail.UpdateRun(runID, Globals.TRRunName, null, null, newCaseIDs);
                    System.Threading.Thread.Sleep(3000);
                    CasesList = trail.GetTests(runID);
                    for (int i = 0; i < CasesList.Count; i++)
                    {
                        if (CasesList[i].Title.Contains(currentScenarioID))
                        {
                            TCaseID = CasesList[i].ID.Value;
                        }
                    }
                    if (TCaseID !=0)
                    {
                        trail.AddResultForCase(runID, TCaseID, TestRail.Types.ResultStatus.Passed, "Test Run Result added automatically", null, null, null, null, null);
                    }
                    else
                    {
                        Console.WriteLine("!!!!!!!! ========= SOMEHOW THE TEST WAS NOT ADDED To the test run ========== !!!!!!!");
                    }

                }

            }
            else //run does not exist, so let's create it, then let's add the scenario to it and also the result to the scenario
            {
                List<ulong> newCaseIDs = new List<ulong>();
                newCaseIDs.Add(caseToUpdateID);
                trail.AddPlanEntry(TestPlanID, TestSuiteID, Globals.TRRunName, null, newCaseIDs);
                //search for test run id (newly added run)
                Test_Runs = trail.GetRuns(TestProjectID);
                for (int i = 0; i < Test_Runs.Count; i++)
                {
                    if (Test_Runs[i].Name == Globals.TRRunName)
                    {
                        runID = Test_Runs[i].ID.Value;
                    }
                }
                if (runID == 0)
                {
                    Console.WriteLine("!!!!!!!! ========= SOMEHOW THE RUN ID is 0  - run was not added to test plan ========== !!!!!!!");
                }
                //search for the test id inside the run
                List<TestRail.Types.Test> CasesList = trail.GetTests(runID);
                ulong TCaseID = 0;
                for (int i = 0; i < CasesList.Count; i++)
                {
                    if (CasesList[i].Title.Contains(currentScenarioID))
                    {
                        TCaseID = CasesList[i].ID.Value;
                    }
                }
                if (TCaseID == 0)
                {
                    Console.WriteLine("!!!!!!!! ========= SOMEHOW THE TC ID is 0  - test was not added to test run when creating the test run ========== !!!!!!!");
                }
                //add result for test inside the test run
                trail.AddResultForCase(runID, TCaseID, TestRail.Types.ResultStatus.Passed, "Test Run Result added automatically", null, null, null, null, null);

            }


            //Test_Runs[1].Name


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
