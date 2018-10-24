using System;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Client.Automation
{
    public class Bootstrap
    {
        private static IWebDriver _chromeDriver;
        public static IWebDriver ChromeDriver 
        { 
            get 
            {
                if(_chromeDriver == null)
                {
                    var capabilities = new DesiredCapabilities();
                    capabilities.SetCapability(CapabilityType.BrowserName, "chrome");
                    capabilities.SetCapability(CapabilityType.Version, "70.0.3538.67");
                    capabilities.SetCapability(CapabilityType.Platform, "LINUX");
                    _chromeDriver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), capabilities);
                    //_chromeDriver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                }
                return _chromeDriver;
            }
        }
    }
}