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
            //initialize TestRail
            TestRailClient trail = new TestRailClient(Globals.TRURL, Globals.TRUser, Globals.TRPass);
            //Get current scenario name and tags and id
            var scenarioTitle = ScenarioContext.Current.ScenarioInfo.Title;
            var scenarioTags = ScenarioContext.Current.ScenarioInfo.Tags;
            var currentScenarioID = scenarioTitle.Split(' ')[0];
            //Get current scenario steps by parsing the feature file
            string FeatureFilePath = Directory.GetCurrentDirectory() + @"\SeleniumSpecFlowTests\Tests\Features\ServiceTests.feature";
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
                //get the ID of the test case we need to update
            while ((casestep < TRCasesList.Count) && (caseToUpdateID == 0))
            {
                if (TRCasesList[casestep].Title.Contains(currentScenarioID))
                {
                    caseToUpdateID = TRCasesList[casestep].ID.Value;
                };
                casestep++;
            };
            //place scenario body (test case steps) in custom field - JSON:
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
                while ((casestep < TRCasesList.Count) && (caseToUpdateID == 0))
                {
                    if (TRCasesList[casestep].Title.Contains(currentScenarioID))
                    {
                        caseToUpdateID = TRCasesList[casestep].ID.Value;

                    };
                    casestep++;
                };
                if (caseToUpdateID == 0)
                {
                    Console.WriteLine("!!!!!!!! ========= SOMEHOW THE TEST CASE WAS NOT ADDED TO TEST RAIL AND CASE ID IS NULL SO CASE WON'T BE ADDED TO TEST RUN ========== !!!!!!!");
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
            List<TestRail.Types.PlanEntry> Test_Runs = trail.GetPlan(TestPlanID).Entries;
            int runIDindex = -1;
            for (int i = 0; i < Test_Runs.Count; i++)
            {
                if (Test_Runs[i].Name == Globals.TRRunName)
                {
                    runIDindex = i;
                }
            }
            if (runIDindex != -1) //run exists so let's add the scenario to it and also add the result to the scenario
            {
                //update existing test run -> add current scenario if not in there already
                ulong runID = trail.GetPlan(TestPlanID).Entries[runIDindex].RunList[0].ID.Value;
                List<TestRail.Types.Test> CasesList = trail.GetTests(runID);
                ulong TCaseID = 0;
                for (int i = 0; i < CasesList.Count; i++)
                {
                    if (CasesList[i].Title.Contains(currentScenarioID))
                    {
                        TCaseID = CasesList[i].ID.Value;
                    }
                }
                if (TCaseID !=0)  //scenario was already added to current test run so we are updating its run result
                {
                    if (ScenarioContext.Current.TestError != null)
                    {
                        trail.AddResult(TCaseID, TestRail.Types.ResultStatus.Failed, "Test Run Result added automatically " + System.Environment.NewLine + ScenarioContext.Current.TestError.Message, null, null, null, null, null);
                    }
                    else
                    {
                        trail.AddResult(TCaseID, TestRail.Types.ResultStatus.Passed, "Test Run Result added automatically ", null, null, null, null, null);
                    }
                }
                else  //scenario was not added to test run (TCaseID was 0) so we are adding it to the test run and setting its run result
                {
                    List<ulong> newCaseIDs = new List<ulong>();
                    //we are going through the list of scenarios inside the Test Run -> for each we then find its counterpart in Test Suite and we create a list of scenario IDs from TEST SUITE
                        //to this list we will then add the one we have just run and re-write the whole test run with the new list
                    foreach (TestRail.Types.Test TestCaseInRun in trail.GetTests(runID))
                    {
                        foreach (TestRail.Types.Case TestCaseInSuite in trail.GetCases(TestProjectID, TestSuiteID, TestAutomationSectionID))
                        {
                            if (TestCaseInSuite.Title == TestCaseInRun.Title)
                            {
                                newCaseIDs.Add(TestCaseInSuite.ID.Value);
                            }
                        }
                    }
                    newCaseIDs.Add(caseToUpdateID);
                        //now we update the test run with the list we have created
                    trail.UpdatePlanEntry(TestPlanID, trail.GetPlan(TestPlanID).Entries[runIDindex].ID, Globals.TRRunName, null, newCaseIDs);

                    System.Threading.Thread.Sleep(3000);

                    //get ID of current test case from the RUN (we have just added it so we know it's there) so we can update the run result (TCaseID is now at 0)
                    CasesList = trail.GetTests(runID);
                    for (int i = 0; i < CasesList.Count; i++)
                    {
                        if (CasesList[i].Title.Contains(currentScenarioID))
                        {
                            TCaseID = CasesList[i].ID.Value;
                        }
                    }
                    if (TCaseID != 0)
                    {
                        if (ScenarioContext.Current.TestError != null)
                        {
                            trail.AddResult(TCaseID, TestRail.Types.ResultStatus.Failed, "Test Run Result added automatically " + System.Environment.NewLine + ScenarioContext.Current.TestError.Message, null, null, null, null, null);
                        }
                        else
                        {
                            trail.AddResult(TCaseID, TestRail.Types.ResultStatus.Passed, "Test Run Result added automatically ", null, null, null, null, null);
                        }
                    }
                    else
                    {
                        Console.WriteLine("!!!!!!!! ========= SOMEHOW THE TEST WAS NOT ADDED To the test run ========== !!!!!!!");
                    }
                }
            }
            else //test run does not exist, so let's create it, then let's add the new scenario to it and also the result to the scenario
            {
                List<ulong> newCaseIDs = new List<ulong>();
                newCaseIDs.Add(caseToUpdateID);
                trail.AddPlanEntry(TestPlanID, TestSuiteID, Globals.TRRunName, null, newCaseIDs);

                System.Threading.Thread.Sleep(3000);
                
                //search for run id (newly added run)
                Test_Runs = trail.GetPlan(TestPlanID).Entries;
                runIDindex = -1;
                for (int i = 0; i < Test_Runs.Count; i++)
                {
                    if (Test_Runs[i].Name == Globals.TRRunName)
                    {
                        runIDindex = i;
                    }
                }
                if (runIDindex == -1)
                {
                    Console.WriteLine("!!!!!!!! ========= SOMEHOW THE RUN ID is 0  - run was not added to test plan ========== !!!!!!!");
                }
                //search for the test id inside the run
                ulong runID = trail.GetPlan(TestPlanID).Entries[runIDindex].RunList[0].ID.Value;
                Console.WriteLine("-----!!!!!!!! Value for the test run ID: " + trail.GetPlan(TestPlanID).Entries[runIDindex].RunList[0].ID.Value.ToString());
                Console.WriteLine("-----!!!!!!!! Value for the test run Name (RunList[0]): " + trail.GetPlan(TestPlanID).Entries[runIDindex].RunList[0].Name);

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

                if (ScenarioContext.Current.TestError != null)
                {
                    trail.AddResult(TCaseID, TestRail.Types.ResultStatus.Failed, "Test Run Result added automatically " + System.Environment.NewLine + ScenarioContext.Current.TestError.Message, null, null, null, null, null);
                }
                else
                {
                    trail.AddResult(TCaseID, TestRail.Types.ResultStatus.Passed, "Test Run Result added automatically ", null, null, null, null, null);
                }
            }
        }
    }
}
