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

        public async Task<bool> SendWhatsappMess(string contactName, string chatMessage, string[] fileContents)
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
                        try
                        {
                            contactElm.Click();
                            await Task.Delay(500);
                            if(fileContents.Length > 0)
                            {
                                List<string> filePath= new List<string>();
                                //Save file to local disk
                                for (int j = 0;j<fileContents.Length;j++)
                                {
                                    var fileItem =await SaveBase64ToLocal(fileContents[j]);
                                    filePath.Add(fileItem);
                                    Console.WriteLine($"Temp file saved {fileItem}");
                                }
                                //Append image
                                await OpenLocalFile(filePath.ToArray());
                                await Task.Delay(1000);
                                var chatboxElm = Driver.FindElementByCssSelector("div[class='_2S1VP copyable-text selectable-text']");
                                //Console.WriteLine("Chat box found");
                                chatboxElm.SendKeys(chatMessage);
                                //_3hV1n yavlE
                                var sendWithImgBut = Driver.FindElementByCssSelector("div[class='_3hV1n yavlE']");
                                //Console.WriteLine("Found button and click");
                                sendWithImgBut.Click();
                                await Task.Delay(5000);
                            }
                            else
                            {
                                var chatboxElm = Driver.FindElementByCssSelector("div[class='_2S1VP copyable-text selectable-text']");
                                //Append text
                                chatboxElm.SendKeys(chatMessage);
                                chatboxElm.SendKeys(Keys.Return);
                            }
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }
            catch{}
            return false;
        }

        private async Task<string> SaveBase64ToLocal(string base64Img)
        {
            String path = Path.GetTempPath();

            //Check if directory exist
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
            }

            var imgPath = Path.Combine(path, $"{Guid.NewGuid().ToString()}.png");

            byte[] imageBytes = Convert.FromBase64String(base64Img);

            File.WriteAllBytes(imgPath, imageBytes);

            return imgPath;
        }

        private async Task OpenLocalFile(string[] filePath)
        {
            try
            {
                var inputs = "";
                for (int i = 0;i<filePath.Length;i++)
                {
                    if(i<filePath.Length-1)
                    {
                        inputs += filePath[i] + "\n";
                    }
                    else
                    {
                        inputs += filePath[i];
                    }
                }
                Console.WriteLine(inputs);
                var attachButton = Driver.FindElementByCssSelector("div[title='Attach']");
                attachButton.Click();
                var inputFile = Driver.FindElementByCssSelector("input[accept='image/*,video/mp4,video/3gpp']");
                inputFile.SendKeys(inputs);
            }
            catch{}
        }
    }
}