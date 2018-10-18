using System.IO;
using System.Reflection;
using OpenQA.Selenium.Chrome;

namespace Client.Automation
{
    public class Bootstrap
    {
        private static ChromeDriver _chromeDriver;
        public static ChromeDriver ChromeDriver 
        { 
            get 
            {
                if(_chromeDriver == null)
                {
                    _chromeDriver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                }
                return _chromeDriver;
            }
        }
    }
}