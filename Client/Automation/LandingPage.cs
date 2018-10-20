using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

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
                string imageContent = "";
                var elm = Driver.FindElementByCssSelector("img[alt='Scan me!']");
                if (elm != null)
                {
                    imageContent = elm.GetAttribute("src");
                }
                return imageContent;
            }
            catch
            {
                return "";
            }
        }
    }
}