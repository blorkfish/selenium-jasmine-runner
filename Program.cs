using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
//using NUnit.Framework;
using ITDM;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace HDWA.SeleniumTestRunner
{
    class TestSuites
    {
        public List<TestSuite> Suites = new List<TestSuite>();
        public int PassCount;
        public int FailCount;
        
        public void WriteToConsole()
        {
            foreach (TestSuite suite in Suites)
                suite.WriteToConsole();

            
            Console.WriteLine(string.Format("Total : {0} Pass : {1} Fail {2}", PassCount + FailCount, PassCount,
                                            FailCount));

            Console.WriteLine("======================================================================");
        }
        public void WriteToStream(StreamWriter sw)
        {
            sw.WriteLine("<testsuites>");
            foreach (TestSuite suite in Suites)
                suite.WriteToStream(sw);
            sw.WriteLine("</testsuites>");
        }
    }
    class TestSuite
    {
        public string Title;
        public int PassCount;
        public int FailCount;
        public List<TestCase> TestCases = new List<TestCase>();

        public void WriteToConsole()
        {
            Console.WriteLine(string.Format("Suite : {2} pass : {0} fail : {1} ", PassCount, FailCount, Title));
            foreach( TestCase tc in TestCases)
                tc.WriteToConsole();
        }
        public void WriteToStream(StreamWriter sw)
        {
            sw.WriteLine(string.Format("<testsuite name=\"{0}\" tests=\"{1}\" failures=\"{2}\">",
                Title, PassCount + FailCount, FailCount
                ));
            
            foreach (TestCase tc in TestCases)
                tc.WriteToStream(sw);

            sw.WriteLine("</testsuite>");
        }
    }

    internal class TestCase
    {
        public bool Passed = false;
        public string Title;
        public string ErrorMessage;
        public string PageName;
        public void WriteToConsole()
        {
            Console.WriteLine(string.Format("   {0}", Title));
            if (!Passed)
                Console.WriteLine(string.Format("       {0}", ErrorMessage));
        }
        public void WriteToStream(StreamWriter sw)
        {
            sw.WriteLine(string.Format("<testcase name=\"{0}\"> classname=\"Jasmine.{1}\"", Title, PageName ));
            if (!Passed)
                sw.WriteLine(string.Format("<error >{0}</error>", ErrorMessage));
            sw.WriteLine("</testcase>");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                ConsoleCmdLine ccl = new ConsoleCmdLine();
                CmdLineString outputFileName = new CmdLineString("output-file", false, "Output File Name - defaults to SeleniumJasmineResults.xml");
                CmdLineString chromeBaseDir = new CmdLineString("chrome-path", false, @"Path to ChromeDriver - defaults to c:\chromedriver_win32_2.2");
                CmdLineString ieBaseDir = new CmdLineString("ie-path", false, @"Path to ChromeDriver - defaults to c:\IEDriverServer_x86_2.34.0");
                CmdLineString inputUrlList = new CmdLineString("input-url-file", false, "Input file containing urls to test - defaults to SpecRunnerList.txt");
                CmdLineParameter runChrome = new CmdLineParameter("chrome", false, "Run Selenium with the chrome driver");
                CmdLineParameter runIE = new CmdLineParameter("ie", false, "Run Selenium with the ie driver");


                ccl.RegisterParameter(outputFileName);
                ccl.RegisterParameter(chromeBaseDir);
                ccl.RegisterParameter(ieBaseDir);
                ccl.RegisterParameter(inputUrlList);
                ccl.RegisterParameter(runChrome);
                ccl.RegisterParameter(runIE);
                ccl.Parse(args);


                string strOutputFileName = !string.IsNullOrEmpty(outputFileName.Value) ? outputFileName.Value : "SeleniumTestRunner.xml";
                string strChromeBaseDir = !string.IsNullOrEmpty(chromeBaseDir.Value) ? chromeBaseDir.Value : @"E:\source\github\selenium-jasmine-runner\chromedriver_win32_2.2";
                string strIEBaseDir = !string.IsNullOrEmpty(ieBaseDir.Value) ? ieBaseDir.Value : @"E:\source\github\selenium-jasmine-runner\IEDriverServer_x64_2.34.0";
                string strSpecRunnerListFile = !string.IsNullOrEmpty(inputUrlList.Value) ? inputUrlList.Value : "SpecRunnerList.txt";

                //switch (args.Length)
                //{
                //    case 0:
                //        Console.WriteLine(
                //            "Please specify a text file as the first argument. this file should contain all specs to run i.e. 'http://localhost:5055/SpecRunner_JourneyBoard.html'");
                //        Console.WriteLine("Please specify the path to chrome base dir as the 2nd argument i.e. " +
                //                          @"C:\source\CWB\PatientFlowTrunk\SharedBinaries\selenium\chromedriver_win_26.0.1383.0");
                //        Console.WriteLine("Output path third arg.");
                //        break;
                //    case 1:
                //        strSpecRunnerListFile = args[0];
                //        break;
                //    case 2:
                //        strSpecRunnerListFile = args[0];
                //        strChromeBaseDir = args[1];
                //        break;
                //    case 3:
                //        strSpecRunnerListFile = args[0];
                //        strChromeBaseDir = args[1];
                //        strOutputFileName = args[2];
                //        break;
                //}

                TestSuites testSuites = new TestSuites();

                List<string> strFileList = new List<string> ();
                using (FileStream fs = new FileStream(strSpecRunnerListFile , FileMode.Open, FileAccess.Read))
                {
                    StreamReader streamReader = new StreamReader(fs);

                    string strLine = streamReader.ReadLine();
                    
                    while (!string.IsNullOrEmpty(strLine))
                    {
                        strFileList.Add(strLine);
                        strLine = streamReader.ReadLine();
                    }
                    
                }

                if (runIE.Exists)
                {
                    using (var driver = new OpenQA.Selenium.IE.InternetExplorerDriver(strIEBaseDir))

                    {
                        foreach (string strSpecRunner in strFileList)
                        {
                            string strPageName = strSpecRunner.Substring(strSpecRunner.LastIndexOf('/') + 1);
                            if (strPageName.IndexOf('.') > 0)
                                strPageName = strPageName.Substring(0, strPageName.IndexOf('.'));

                            RunAllJasmineTestsAndReportTrivialReporter(driver, strSpecRunner, strPageName, "IE", ref testSuites);
                        }

                    }
                }

                if (runChrome.Exists)
                {
                    using (var driver = new OpenQA.Selenium.Chrome.ChromeDriver(strChromeBaseDir))
                    {
                        foreach (string strSpecRunner in strFileList)
                        {
                            string strPageName = strSpecRunner.Substring(strSpecRunner.LastIndexOf('/') + 1);
                            if (strPageName.IndexOf('.') > 0)
                                strPageName = strPageName.Substring(0, strPageName.IndexOf('.'));

                            RunAllJasmineTestsAndReportTrivialReporter(driver, strSpecRunner, strPageName, "Chrome", ref testSuites);
                        }

                    }
                }

                Console.WriteLine("-----------");

                testSuites.WriteToConsole();

                using (FileStream fs = new FileStream(strOutputFileName, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(fs);

                    testSuites.WriteToStream(streamWriter);

                    streamWriter.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("Unhandled Exception " + ex.ToString());
                Console.WriteLine("");
                Console.WriteLine(ex.ToString());
            }


        }


        private static void RunAllJasmineTestsAndReportTrivialReporter(IWebDriver driver, string TEST_PAGE, string strPageName, string browser, ref TestSuites testSuites )
        {
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(90));
            driver.Navigate().GoToUrl(TEST_PAGE);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(90));

            bool bFound = wait.Until(d => { return 
                (d.FindElement(By.CssSelector("span.finished-at")).Text.Length > 0); 
            });

            string duration = driver.FindElement(By.CssSelector("span.finished-at")).Text;

            IWebElement showPassed = driver.FindElement(By.Id("__jasmine_TrivialReporter_showPassed__"));
            if (!showPassed.Selected)
                showPassed.Click();

            Thread.Sleep(3000);

            var suites = driver.FindElements(By.CssSelector("div.suite"));
            foreach (var suite in suites)
            {
                // suite pass / fail
                TestSuite testSuite = new TestSuite();

                var description = suite.FindElement(By.CssSelector("a.description"));

                testSuite.Title = browser + " : " + description.Text;

                var specs = suite.FindElements(By.CssSelector("div.spec"));

                foreach (var spec in specs)
                {
                    TestCase testCase = new TestCase();
                    var descr = spec.FindElement(By.CssSelector("a.description"));
                    testCase.Title = descr.Text;
                    testCase.PageName = strPageName;

                    string strSpecTitle = descr.Text;
                    
                    var specClass = spec.GetAttribute("class");
                    if (specClass.Contains("passed"))
                    {
                        testCase.Passed = true;
                        testSuite.PassCount++;
                        testSuites.PassCount++;
                    }
                    else
                    {
                        testSuite.FailCount++;
                        testSuites.FailCount++;
                        var resultMessage = spec.FindElement(By.CssSelector("div.resultMessage"));
                        testCase.ErrorMessage = resultMessage.Text;
                    }

                    testSuite.TestCases.Add(testCase);
                }

                testSuites.Suites.Add(testSuite);
            }

            

        }
    }
}

