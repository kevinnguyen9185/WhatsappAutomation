using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace Client.Automation
{
    public class Page
    {
        const string JS_DROP_FILE = "for(var b=arguments[0],k=arguments[1],l=arguments[2],c=b.ownerDocument,m=0;;){var e=b.getBoundingClientRect(),g=e.left+(k||e.width/2),h=e.top+(l||e.height/2),f=c.elementFromPoint(g,h);if(f&&b.contains(f))break;if(1<++m)throw b=Error('Element not interractable'),b.code=15,b;b.scrollIntoView({behavior:'instant',block:'center',inline:'center'})}var a=c.createElement('INPUT');a.setAttribute('type','file');a.setAttribute('style','position:fixed;z-index:2147483647;left:0;top:0;');a.onchange=function(){var b={effectAllowed:'all',dropEffect:'none',types:['Files'],files:this.files,setData:function(){},getData:function(){},clearData:function(){},setDragImage:function(){}};window.DataTransferItemList&&(b.items=Object.setPrototypeOf([Object.setPrototypeOf({kind:'file',type:this.files[0].type,file:this.files[0],getAsFile:function(){return this.file},getAsString:function(b){var a=new FileReader;a.onload=function(a){b(a.target.result)};a.readAsText(this.file)}},DataTransferItem.prototype)],DataTransferItemList.prototype));Object.setPrototypeOf(b,DataTransfer.prototype);['dragenter','dragover','drop'].forEach(function(a){var d=c.createEvent('DragEvent');d.initMouseEvent(a,!0,!0,c.defaultView,0,0,0,g,h,!1,!1,!1,!1,0,null);Object.setPrototypeOf(d,null);d.dataTransfer=b;Object.setPrototypeOf(d,DragEvent.prototype);f.dispatchEvent(d)});a.parentElement.removeChild(a)};c.documentElement.appendChild(a);a.getBoundingClientRect();return a;";
        public Page()
        {
            _driver = Bootstrap.ChromeDriver;
        }

        public bool IsRemoteDriver { get {
            return Driver.GetType().FullName == typeof(RemoteWebDriver).FullName;
        }}
        private readonly IWebDriver _driver;

        public string PageTitle { get; set; }

        public IWebDriver Driver { get {return _driver;} }
        public IJavaScriptExecutor JavaScriptExecutor
		{
			get { return (IJavaScriptExecutor) _driver; }
		}
        
        public void WaitForLoad()
        {
            try
            {
                Wait()
                    .Until(_driver => _driver.Title.Contains(this.PageTitle));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Wait for load failed for {this.PageTitle}");
            }
        }

        public void WaitForElement(IWebElement element)
		{
			Wait()
				.Until(driver => element.Displayed);
		}

        public IWebElement WaitForElementExisted(string cssSelector)
		{
			return Wait()
				.Until(ExpectedConditions.ElementExists(By.CssSelector(cssSelector)));
		}

        public void WaitForJquery()
		{
			Wait()
                .Until(_driver => (bool)JavaScriptExecutor.ExecuteScript(" return !!window.jQuery && window.jQuery.active == 0"));
		}

        public void GoTo(string url)
        {
            _driver.Url = url;
            WaitForLoad();
        }

        public void RefreshPage()
		{
			_driver.Url = _driver.Url;
			_driver.Navigate();
            WaitForLoad();
		}

        public WebDriverWait Wait(int wait = 180, IList<Type> ignoreExceptionTypes = null)
		{
			var webDriverWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(wait));
			webDriverWait.IgnoreExceptionTypes(ignoreExceptionTypes?.ToArray() ?? new [] {
				typeof(StaleElementReferenceException)
			});

			return webDriverWait;
		}

        public async Task DropFile(IWebElement target, string filePath, double offsetX = 0, double offsetY = 0)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            IWebDriver driver = ((RemoteWebElement)target).WrappedDriver;
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;

            IWebElement input = (IWebElement)jse.ExecuteScript(JS_DROP_FILE, target, offsetX, offsetY);
            input.SendKeys(filePath);
        }

        public async Task TakeScreenShot()
        {
            if(IsRemoteDriver)
            {
                Console.WriteLine("Taking screen shot");
                String dockerpath = "/home/seluser";
                (Driver as RemoteWebDriver).GetScreenshot().SaveAsFile($"{dockerpath}/Screenshot_{DateTime.Now}.png",ScreenshotImageFormat.Png);
            }
        }
    }
}