using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using static OpenQA.Selenium.Support.UI.ExpectedConditions;

namespace WebdriverTimeoutsTutorial
{
    [TestClass]
    public class MixingWaitsTest
    {
        private IWebDriver _driver;
        private const string URI = "https://the-internet.herokuapp.com/dynamic_loading/2";
        private By elementId = By.Id("finish");
        Stopwatch _stopwatch = new Stopwatch();

        public void Setup()
        {
            _driver = WebDriverCreator.RemoteInitialize();
            //go to a url that contains a dynamically loading page element
            _driver.Navigate().GoToUrl(URI);
            //click the start button
            _driver.FindElement(By.TagName("button")).Click();
        }

        public void Teardown()
        {
            _driver.Close();
            _driver.Quit();
        }


        [TestMethod]
        public void MixingExplicitAndImplicitWaits()
        {
            
            IWebElement myElement;
            for (int i = 0; i < 10; i++)
            {
                Setup();       
                Trace.WriteLine(string.Format("Loop Iteration:{0}", i));
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(0 + i));
                Trace.WriteLine($"Explicit wait timeout:{wait.Timeout.Seconds}s");

                var implicitWaitTime = TimeSpan.FromSeconds(5 + i);
                _driver.Manage().Timeouts().ImplicitlyWait(implicitWaitTime);
                //String interpolation introduced in C# 6.0
                Trace.WriteLine($"Implicit wait timeout:{implicitWaitTime.Seconds}s");
                _stopwatch.Start();
                try
                {
                    myElement = wait.Until(ElementIsVisible(elementId));

                }
                catch (Exception e)
                {
                    //do nothing
                }
                _stopwatch.Stop();
                Trace.WriteLine($"Time Elapsed for element identification:{_stopwatch.Elapsed.Seconds}s");
                Trace.WriteLine("-----------------");
                _stopwatch.Reset();
                Teardown();
            }
        }

        [TestMethod]
        public void ExplicitWait2()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _stopwatch.Start();
            IWebElement myDynamicElement = wait.Until<IWebElement>((d) =>
            {
                return d.FindElement(elementId);
            });
            _stopwatch.Stop();
            Trace.WriteLine($"ExplicitWait2-Time Elapsed for element identification:{_stopwatch.Elapsed.TotalSeconds}s");
        }

        [TestMethod]
        public void Time_WebDriverWait()
        {
            IClock clock = new SystemClock();
            var wait = new WebDriverWait(clock,_driver,TimeSpan.FromSeconds(10),TimeSpan.FromSeconds(5));
            _stopwatch.Start();
            var myElement = wait.Until(ElementIsVisible(elementId));
            _stopwatch.Stop();
            Trace.WriteLine($"Time_WebDriverWait-Time Elapsed for element identification:{_stopwatch.Elapsed.TotalSeconds}s");
        }

        [TestMethod]
        public void Clean_WebDriverWait()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _stopwatch.Start();
            var myElement = wait.Until(ElementExists(elementId));
            _stopwatch.Stop();
            Trace.WriteLine($"Clean_WebDriverWait-Time Elapsed for element identification:{_stopwatch.Elapsed.TotalSeconds}s");
        }
    }
}
