using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
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
                var elm = Driver.FindElement(By.CssSelector("span[data-icon]"));
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
                var contactList = Driver.FindElements(By.CssSelector("div[class='_2wP_Y']"));
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
                var contactList = Driver.FindElements(By.CssSelector("div[class='_2wP_Y']"));
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
                                await Task.Delay(500);
                                var chatboxElm = Driver.FindElement(By.CssSelector("div[class='_2S1VP copyable-text selectable-text']"));
                                //Console.WriteLine("Chat box found");
                                chatboxElm.SendKeys(Keys.Enter);
                                await Task.Delay(2000);
                                chatboxElm = Driver.FindElement(By.CssSelector("div[class='_2S1VP copyable-text selectable-text']"));
                                chatboxElm.SendKeys(chatMessage);
                                await Task.Delay(500);
                                chatboxElm.SendKeys(Keys.Enter);
                                //_3hV1n yavlE
                                // var sendWithImgBut = Driver.FindElementByCssSelector("div[class='_3hV1n yavlE']");
                                // //Console.WriteLine("Found button and click");
                                // sendWithImgBut.Click();
                                //Driver.GetScreenshot().SaveAsFile($"/Users/kevinng/Ansarada/Selenium_docker/Chuanbigui_{Guid.NewGuid().ToString()}.png",ScreenshotImageFormat.Png);
                                
                            }
                            else
                            {
                                var chatboxElm = Driver.FindElement(By.CssSelector("div[class='_2S1VP copyable-text selectable-text']"));
                                //Append text
                                chatboxElm.SendKeys(chatMessage);
                                chatboxElm.SendKeys(Keys.Return);
                            }
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            return false;
                        }
                    }
                }
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        private async Task<string> SaveBase64ToLocal(string base64Img)
        {
            String hostpath = Path.Combine(System.Environment.GetEnvironmentVariable("SEL_SHARED_FOLDER"), "robot_images");
            String dockerpath = "/home/seluser";

            //Check if directory exist
            if (!System.IO.Directory.Exists(hostpath))
            {
                System.IO.Directory.CreateDirectory(hostpath); //Create directory if it doesn't exist
            }

            var imgFileName = $"{Guid.NewGuid().ToString()}.png";
            var imgPath = Path.Combine(hostpath, imgFileName);

            byte[] imageBytes = Convert.FromBase64String(base64Img);

            File.WriteAllBytes(imgPath, imageBytes);

            if (IsRemoteDriver)
            {
                return Path.Combine(dockerpath, imgFileName);
            }
            else
            {
                return imgPath;
            }
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
                //Console.WriteLine(inputs);
                var attachButton = Driver.FindElement(By.CssSelector("div[title='Attach']"));
                attachButton.Click();
                await Task.Delay(1000);
                if(IsRemoteDriver)
                {
                    //Console.WriteLine("Using remote");
                    var allowsDetection = Driver as IAllowsFileDetection;
                    if(allowsDetection!=null)
                    {
                        allowsDetection.FileDetector = new LocalFileDetector();
                        //Console.WriteLine("Set allow detection");
                    }
                }
                var inputFile = Driver.FindElement(By.CssSelector("input[accept='image/*,video/mp4,video/3gpp']"));
                //Console.WriteLine(inputFile.TagName);
                inputFile.SendKeys(inputs);
                //inputFile.SendKeys("/home/seluser/2ngo-thanh-van.jpg");
                ((RemoteWebDriver)Driver).GetScreenshot().SaveAsFile($"/Users/kevinng/Ansarada/Selenium_docker/Draghinh_{Guid.NewGuid().ToString()}.png",ScreenshotImageFormat.Png);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}