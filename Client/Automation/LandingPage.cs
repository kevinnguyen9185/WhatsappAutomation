using System;
using System.IO;
using System.Reflection;
using OpenQA.Selenium.Chrome;

namespace Client.Automation
{
    public class LandingPage
    {
        private readonly ChromeDriver _driver;
        public LandingPage()
        {
            _driver = Bootstrap.ChromeDriver;
            _driver.Url = "https://web.whatsapp.com/";
            _driver.Navigate();
        }

        public string GetQRCodeImage()
        {
            var imageContent = _driver.FindElementByCssSelector("img[alt='Scan me!']").GetAttribute("src");
            return imageContent;
        }
    }
}