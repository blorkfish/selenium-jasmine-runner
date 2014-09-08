using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HDWA.SeleniumTestRunner;

namespace SeleniumJasmineRunner
{
    internal interface ITestReporter
    {
        void WriteTestCaseToStream(TestCase testCase, StreamWriter sw, TestSuite testSuite);
        void WriteTestSuiteToStream(TestSuite testCase, StreamWriter sw);
        void WriteTestSuiteListToStream(TestSuites testCase, StreamWriter sw);
    }

    class TestReporterFactory
    {
        public static ITestReporter GetTestReporter(string reporter)
        {
            switch (reporter)
            {
                case "jenkins":
                    return new JenkinsReporter();
            }
            return new TeamCityReporter();
        }
    }

    internal class JenkinsReporter : ITestReporter
    {
        public void WriteTestCaseToStream(TestCase testCase, StreamWriter sw, TestSuite testSuite)
        {
            sw.WriteLine(string.Format("<testcase name=\"{0}\"> classname=\"Jasmine.{1}\"", testCase.Title, testCase.PageName));
            if (!testCase.Passed)
                sw.WriteLine(string.Format("<error >{0}</error>", testCase.ErrorMessage));
            sw.WriteLine("</testcase>");
        }

        public void WriteTestSuiteToStream(TestSuite testSuite, StreamWriter sw)
        {
            sw.WriteLine(string.Format("<testsuite name=\"{0}\" tests=\"{1}\" failures=\"{2}\">",
                testSuite.Title, testSuite.PassCount + testSuite.FailCount, testSuite.FailCount
                ));

            foreach (TestCase tc in testSuite.TestCases)
                WriteTestCaseToStream(tc, sw, testSuite);

            sw.WriteLine("</testsuite>");
        }


        public void WriteTestSuiteListToStream(TestSuites testSuiteList, StreamWriter sw)
        {
            sw.WriteLine("<testsuites>");
            foreach (TestSuite suite in testSuiteList.Suites)
                WriteTestSuiteToStream(suite, sw);
            sw.WriteLine("</testsuites>");
        }
    }

    internal class TeamCityReporter : ITestReporter
    {
        public void WriteTestCaseToStream(TestCase testCase, StreamWriter sw, TestSuite testSuite)
        {
            string fulltestName = testSuite.Title + " => " + testCase.Title;

            sw.WriteLine(string.Format("##teamcity[testStarted name='{0}' captureStandardOutput='true']", fulltestName));
            if (!testCase.Passed)
                sw.WriteLine(string.Format("##teamcity[testFailed name='{0}' message='{1}']", fulltestName, testCase.ErrorMessage.Replace('\'', '"')));
            sw.WriteLine(string.Format("##teamcity[testFinished name='{0}' duration='{1}']", fulltestName, testCase.DurationMilliseconds));
        }

        public void WriteTestSuiteToStream(TestSuite testSuite, StreamWriter sw)
        {
            sw.WriteLine(string.Format("##teamcity[testSuiteStarted name='{0}']", testSuite.Title//, testSuite.PassCount + testSuite.FailCount, testSuite.FailCount
                ));

            foreach (TestCase tc in testSuite.TestCases)
                WriteTestCaseToStream(tc, sw, testSuite);

            sw.WriteLine(string.Format("##teamcity[testSuiteFinished name='{0}']", testSuite.Title//, testSuite.PassCount + testSuite.FailCount, testSuite.FailCount
                ));
        }

        public void WriteTestSuiteListToStream(TestSuites testSuiteList, StreamWriter sw)
        {
            //sw.WriteLine("<testsuites>");
            foreach (TestSuite suite in testSuiteList.Suites)
                WriteTestSuiteToStream(suite, sw);
            //sw.WriteLine("</testsuites>");
        }
    }


}
