using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace SpecFlowTests
{
    class Calculator
    {
        private readonly List<int> _inputs = new List<int>();
        private int _result = 0;

        public void EnterInput(int input)
        {
            _inputs.Add(input);
        }

        public void ComputeSum()
        {
            // Add all inputs together
            _result = _inputs.Sum();
        }

        public int GetResult()
        {
            return _result;
        }

    }

    [Binding]
    public class CalculatorSteps
    {
        private Calculator _calculator = new Calculator();
        
        [Given(@"I have entered (.*) into the calculator")]
        public void GivenIHaveEnteredIntoTheCalculator(int p0)
        {
            _calculator.EnterInput(p0);
        }

        [When(@"I press add")]
        public void WhenIPressAdd()
        {
            _calculator.ComputeSum();
        }

        [Then(@"the result should be (.*) on the screen")]
        public void ThenTheResultShouldBeOnTheScreen(int p0)
        {
            Assert.AreEqual(p0, _calculator.GetResult());
        }
    }
}
