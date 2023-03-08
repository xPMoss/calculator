using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using calculator;

namespace UnitTestProject.Test
{
    [TestClass()]
    public class UnitTest
    {
        [TestMethod()]
        public void TestCalculate01()
        {
            List<char> operatorList = new List<char>(2) { 'x', '+' }; // <- STORE OPERATORS TO USE IN CALC
            List<double> inputNumberList = new List<double>(3) { 9, 9, 3 }; // <- STORE NUMBERS TO USE IN CALC

            double result = MainPage.Calculate(operatorList, inputNumberList);

            //Assert.AreEqual(100, result);
            Assert.AreEqual(84, result);

        }

        [TestMethod()]
        public void TestCtoF01()
        {

            string type = "c";
            double number = 32;

            double convertResult = MainPage.CtoF(number, type);



            //Assert.AreEqual(84, convertResult);
            Assert.AreEqual(89.60000, convertResult);



        }
    }
}
