using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Client.Automation
{
    public class ChatPage : Page
    {
        public bool IsLogin 
        { 
            get
            {
                return CheckLoginStatus();
            }
        }
        public ChatPage(string title)
        {
            PageTitle = title;
        }

        public bool CheckLoginStatus()
        {
            try
            {
                var elm = Driver.FindElementByCssSelector("span[data-icon]");
                return elm != null;
            }
            catch(NoSuchElementException ex)
            {
                return false;
            }
        }

        public List<string> GetContactList()
        {
            List<string> lstContact = new List<string>();
            try
            {
                var contactList = Driver.FindElementsByCssSelector("div[class='_2wP_Y']");
                for (int i = 0;i<contactList.Count;i++)
                {
                    var contactElm = contactList[i];
                    try
                    {
                        var contactName = contactElm.FindElement(By.CssSelector("span[class='_1wjpf']"));
                        lstContact.Add(contactName.Text);
                        //Console.WriteLine(contactName.Text);
                    }
                    catch{}
                }
            }
            catch
            {

            }
            return lstContact;
        }

        public async Task<bool> SendWhatsappMess(string contactName, string chatMessage)
        {
            try
            {
                var contactList = Driver.FindElementsByCssSelector("div[class='_2wP_Y']");
                for (int i = 0;i<contactList.Count;i++)
                {
                    var contactElm = contactList[i];
                    var htmlContent = contactElm.GetAttribute("innerHTML");
                    if(htmlContent.Contains(contactName))
                    {
                        contactElm.Click();
                        await Task.Delay(500);
                        var chatboxElm = Driver.FindElementByCssSelector("div[class='_2S1VP copyable-text selectable-text']");
                        var fileDropArea = Driver.FindElementByCssSelector("div[class='_2nmDZ']");
                        //Append text
                        chatboxElm.SendKeys(chatMessage);
                        await Task.Delay(500);
                        //Append image
                        await DropFile(fileDropArea, "/Users/kevinng/Desktop/Test.png");
                        await DropFile(fileDropArea, "/Users/kevinng/Desktop/Test.png");
                        await DropFile(fileDropArea, "/Users/kevinng/Desktop/Test.png");
                        await Task.Delay(1000);
                        chatboxElm.SendKeys(Keys.Return);
                        return true;
                    }
                }
            }
            catch{}
            return false;
        }
    }
}