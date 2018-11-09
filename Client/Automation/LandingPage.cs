using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Serilog;

namespace Client.Automation
{
    public class LandingPage : Page
    {
        public string QRCode 
        { 
            get
            {
                return GetQRCodeImage();
            }
        }
        public LandingPage(string title)
        {
            PageTitle = title;
            GoTo("https://web.whatsapp.com/");
            WaitForLoad();
        }

        public string GetQRCodeImage()
        {
            try
            {
                Log.Information("refresh QR code");
                this.RefreshPage();
                WaitForElementExisted("img[alt='Scan me!']");
                string imageContent = "";
                var elm = Driver.FindElement(By.CssSelector("img[alt='Scan me!']"));
                if (elm != null)
                {
                    imageContent = elm.GetAttribute("src");
                }
                Log.Information("retrieve QR code and send back");
                return imageContent;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
    }
}