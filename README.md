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








have fun,  
Blorkfish.