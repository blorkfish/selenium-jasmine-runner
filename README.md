selenium-jasmine-runner
=======================

A selenium based jasmine reporter for Jenkins and TeamCity.

Introduction
---

When running jasmine unit tests on a Continuous Integration build server, most frameworks will use PhantomJS to run their unit tests.  

But what happens if you want to actually run your tests in Firefox, Chrome or even Internet Explorer - to furthur safeguard your code against browser-specific errors ?

This is where Selenium comes in.  Selenium simply automates a web-browser.  Using Selenium, we can open up a browser, specify a url, and then wait for DOM elements to be rendered on the page.

This project does exactly that for jasmine unit tests.

Our projects are heavily ASP.NET based - meaning that we are not simply dealing with basic html pages - we are generating pages from multiple ASP.NET Views, Layout Views and Shared Views.  Also, we are using the bundling mechanism in ASP.NET to bundle css and javascript.  For this reason, we are not able to run jasmine tests in a simple web-page, we need to target a TestController and TestView on the actual server.

This project is an executable that can be run on the CI server and is driven by multiple command line parameters.  It is capable of running the Selenium Driver for Internet Explorer, Chrome and FireFox.

Command line parameters
---

    -chrome-path    : The path to chromedriver.exe ( The selenium chrome driver )
        default     : c:\selenium\chromedriver_win32_2.2
    -ie-path        : The path to IEDriverServer.exe ( The selenium IE driver )
        default     : c:\selenium\IEDriverServer_x86_2.34.0
    -input-url-file : An input file containing a list of urls.
        default     : SpecRunnerList.txt
    -output-file    : An output file name for Jenkins results 
        default     : SeleniumJasmineResults.xml
    -chrome         : A flag indicating whether or not to run tests in Chrome
        default     : false
    -ie             : A flag indicating whether or not to run tests in Internet Explorer
        default     : false
    -firefox        : A flag indicating whether or not to run tests in FireFox
        default     : false
    -timeout        : A timeout value ( seconds ) to wait for the tests to finish
        default     : 90 seconds
    -reporter       : The command-line reporter to use - specify either [ jenkins | teamcity ]
        default     : teamcity
    -reporter-input : The reporter input type - use either [ trivialreporter | logreporter ]
                    : For jasmine < 2.0 use trivialreporter as input
                    : For jasmine >= 2.0 use logreporter as input

Setting up Selenium
---

In order to use this executable, you will need a Selenium Driver to drive either IE or Chrome.  FireFox has an extension that needs to be installed.

**Chrome Driver** 

Download the Chrome Driver from here: [chromedriver.storage.googleapis.com/index.html](http://chromedriver.storage.googleapis.com/index.html)
Unzip the chromedriver_win32.zip file, and use this path as the --chrome-path parameter on the command line.

**Internet Explorer Driver**

Download the Internet Explorer driver from this page [www.seleniumhq.org/download/](http://www.seleniumhq.org/download/).  
Look for the Internet Explorer Driver Server section and follow the link.

**FireFox Extension**

For FireFox, you will need to install the Selenium IDE extension - which at the time of writing is at version 2.8.0.

Jasmine Versions
---

**Jasmine 1.3**

For Jasmine tests < 2.0, this executable uses the TrivialReporter to report unit tests.  Note the code included under the /TestPages directory, or TestPages project:  

    /TestPages/teamcity_reporter_1.3.html

**Jasmien 2.0**

For Jasmine tests >= 2.0, this executable uses a modified version of teamcity_reporter, as the TrivialReporter has been deprecated.  Have a look a the page

    /TestPages/teamcity_reporter_2.0.html
    
The modified version of the teamcity_reporter can be found at 

    /TestPages/teamcity_reporter.js

You will need to include the script at the bottom of the page, as well as the two reporting divs at the bottom of the page as well:

    <script type="application/javascript">
        window.api = new jasmine.JsApiReporter({});
        jasmine.getEnv().addReporter(window.api);

        window.tcapi = new jasmineReporters.TeamCityReporter({});
        jasmine.getEnv().addReporter(window.tcapi);

        var env = jasmine.getEnv();
        var test = "test";

        var resultsDone = false;
        
        window.setInterval(function() {
            if (window.api.finished && !resultsDone) {

                var suites = window.api.suites();
                var test = window.tcapi;

                var logItems = window.tcapi.logItems;

                for (var i = 0; i < logItems.length; i++) {
                    // non jquery way of appending dom elements
                    var logElement = document.getElementById('teamCityReporterLog');
                    var node = document.createElement('div');
                    node.setAttribute('class','logentry');
                    var textNode = document.createTextNode(logItems[i]);
                    node.appendChild(textNode);
                    logElement.appendChild(node);

                }

                resultsDone = true;
                var doneFlag = document.getElementById('teamCityResultsDone');
                var doneText = document.createTextNode('done');
                doneFlag.appendChild(doneText);
            }
        }, 5000);
    </script>

    <div id="teamCityReporterLog"></div>
    <div id="teamCityResultsDone"></div>
    



have fun,  
Blorkfish.